using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookLAB.Infrastructure.Services
{
    /// <summary>
    /// AI Service toàn diện cho Booking phòng Lab
    /// - Parsing thông tin từ natural language
    /// - Learning từ lịch sử user (preferences)
    /// - Detect conflicts và suggest alternatives
    /// - Confidence scoring
    /// </summary>
    public class AdvancedAIBookingService : IAIBookingService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AdvancedAIBookingService(
            IConfiguration configuration,
            HttpClient httpClient,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _apiKey = configuration["DeepSeek:ApiKey"];
            _httpClient = httpClient;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Main entry point: Parse user prompt + suggest best booking options
        /// </summary>
        public async Task<AIBookingResponse> ParseAndSuggestAsync(string userPrompt, CancellationToken ct = default)
        {
            try
            {
                // 1. Extract raw data từ Gemini
                var rawParsing = await ParseBookingCommandAsync(userPrompt, ct);

                if (rawParsing == null)
                {
                    return new AIBookingResponse
                    {
                        Status = AIResponseStatus.ParseError,
                        Message = "Không thể hiểu được yêu cầu của bạn. Vui lòng nói rõ hơn.",
                        Confidence = 0
                    };
                }

                // 2. Normalize & Validate parsed data
                var normalized = await NormalizeAndEnrichAsync(rawParsing, ct);

                // 3. Detect conflicts với lịch của user
                var conflicts = await DetectConflictsAsync(normalized, ct);

                // 5. Generate alternatives nếu có conflict
                var suggestions = await GenerateAlternativesAsync(normalized, conflicts, ct);

                // 6. Build response with confidence scoring
                var response = BuildResponse(normalized, suggestions, conflicts);

                return response;
            }
            catch (Exception ex)
            {
                return new AIBookingResponse
                {
                    Status = AIResponseStatus.SystemError,
                    Message = $"Lỗi hệ thống: {ex.Message}",
                    Confidence = 0
                };
            }
        }

        /// <summary>
        /// Giai đoạn 1: Call Gemini để extract thông tin
        /// </summary>
        private async Task<AISchedulingResult?> ParseBookingCommandAsync(string userPrompt, CancellationToken ct)
        {
            // Build context từ database
            var context = await BuildAIContextAsync(ct);

            string systemInstructions = $@"
Bạn là trợ lý ảo BookLAB chuyên parse yêu cầu đặt phòng Lab.
Ngày hôm nay: {DateTime.Now:dd/MM/yyyy HH:mm}

=== DANH SÁCH HỆ THỐNG ===
Các phòng Lab:
{context.RoomContext}

Các loại Slot tại campus này:
{context.SlotTypesContext}

Mục đích sử dụng:
{context.PurposeTypesContext}

=== QUY TẮC PARSING (BẮT BUỘC) ===

1. **ActionType**: Luôn là 'CreateBooking'

2. **RoomCode** (phòng cần đặt):
   - Ví dụ: AL-202, LAB-301
   - Nếu user nói 'phòng đã quen' hoặc 'bất kỳ phòng nào', để null
   - Nếu không nhắc tới, để null

3. **Dates & Times** (TỚI QUAN TRỌNG):
   
   CÓ 3 CÁCH ĐẶT GIỜ:
   
   a) **Cách A - Slot cố định** (user nói: 'slot 1,2' hoặc 'buổi sáng'):
      - SlotTypeName: Tên loại slot (ví dụ 'Morning Slots')
      - Slots: Mảng index của slot (ví dụ [1, 2])
      - StartTime, EndTime: NULL
      - Date: Ngày đặt (yyyy-MM-dd)
   
   b) **Cách B - Giờ tự do** (user nói: 'từ 8h đến 12h' hoặc 'trong 4 tiếng'):
      - StartTime, EndTime: HH:mm (ví dụ '08:00', '12:00')
      - Slots, SlotTypeName: NULL
      - Date: Ngày đặt (yyyy-MM-dd)
   
   c) **Cách C - Relative dates** (user nói: 'mai', 'tuần sau', 'hôm kia'):
      - Tính toán ngày cụ thể từ hôm nay {DateTime.Now:dd/MM/yyyy}
      - Ví dụ: 'mai' = {DateTime.Now.AddDays(1):yyyy-MM-dd}
      - 'tuần sau' = {DateTime.Now.AddDays(7):yyyy-MM-dd}
      - 'hôm kia' = {DateTime.Now.AddDays(2):yyyy-MM-dd}
      - Nếu user nói 'thứ X' (ví dụ 'Thứ 3 tuần sau'), hãy tính ngày thứ 3 tuần tới

4. **StudentCount**: Số lượng sinh viên
   - Mặc định: 1
   - Ví dụ: user nói '5 người' → 5

5. **PurposeType**: Chọn mục đích phù hợp
   - PHẢI là 1 giá trị trong danh sách đã cho
   - Nếu không rõ, chọn generic nhất

6. **Recurring Pattern**:
   - RecurringCount: Số tuần lặp lại (1-4, mặc định 1)
   - Ví dụ: 'hàng tuần trong 3 tuần' → RecurringCount = 3
   - Nếu user nói 'tuần tới' (1 lần) → RecurringCount = 1

7. **Confidence & Issues**:
   - Luôn trả về confidence score (0-100) về mức độ chắc chắn
   - Issues: Mảng các cảnh báo (phòng không có, giờ không hợp lệ, etc)

=== ĐỊNH DẠNG OUTPUT ===
Chỉ trả về JSON, không giải thích thêm:
{{
  ""actionType"": ""CreateBooking"",
  ""roomCode"": ""AL-202"" | null,
  ""date"": ""2026-04-30"",
  ""slotTypeName"": ""Morning Slots"" | null,
  ""slots"": [1, 2] | null,
  ""startTime"": ""08:00"" | null,
  ""endTime"": ""12:00"" | null,
  ""studentCount"": 5,
  ""purposeType"": ""Class Lecture"",
  ""recurringCount"": 1,
  ""confidence"": 85,
  ""issues"": []
}}
";

            // DeepSeek API Request
            var requestBody = new
            {
                model = "deepseek-reasoner",
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = systemInstructions
                    },
                    new
                    {
                        role = "user",
                        content = userPrompt
                    }
                },
                stream = false,
                reasoning_effort = "high"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.deepseek.com/chat/completions"
            );

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Content = content;

            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"DeepSeek API error: {response.StatusCode} - {error}");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);

            var text = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var cleanJson = text!
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            return JsonSerializer.Deserialize<AISchedulingResult>(cleanJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            });
        }
        /// <summary>
        /// Giai đoạn 2: Normalize và enrich data
        /// - Convert relative dates thành absolute dates
        /// - Resolve room code → room ID
        /// - Resolve slot names → slot indices
        /// - Validate time ranges
        /// </summary>
        private async Task<NormalizedBookingRequest> NormalizeAndEnrichAsync(
            AISchedulingResult parsed,
            CancellationToken ct)
        {
            var normalized = new NormalizedBookingRequest();

            // 1. Resolve room
            if (!string.IsNullOrEmpty(parsed.RoomCode))
            {
                var room = await _unitOfWork.Repository<LabRoom>().Entities
                    .FirstOrDefaultAsync(r => r.RoomNo == parsed.RoomCode, ct);

                if (room != null)
                {
                    normalized.LabRoomId = room.Id;
                    normalized.RoomName = room.RoomName;
                }
            }

            // 2. Resolve purpose type
            if (!string.IsNullOrEmpty(parsed.PurposeType))
            {
                var purpose = await _unitOfWork.Repository<PurposeType>().Entities
                    .FirstOrDefaultAsync(p => p.PurposeName.ToLower() == parsed.PurposeType.ToLower(), ct);

                if (purpose != null)
                {
                    normalized.PurposeTypeId = purpose.Id;
                }
            }

            // 3. Handle dates & times
            if (DateTime.TryParse(parsed.Date, out var parsedDate))
            {
                normalized.BaseDate = parsedDate.Date;
            }

            // 4. Handle slot-based booking
            if (parsed.Slots != null && parsed.Slots.Any())
            {
                var slotType = await _unitOfWork.Repository<SlotType>().Entities
                    .Include(s => s.SlotFrames)
                    .FirstOrDefaultAsync(s => s.Name == parsed.SlotTypeName, ct);

                if (slotType != null)
                {
                    normalized.SlotTypeId = slotType.Id;
                    normalized.SlotIndices = parsed.Slots;

                    // Extract time range từ slot frames
                    var frames = slotType.SlotFrames
                        .Where(f => parsed.Slots.Contains(f.OrderIndex))
                        .OrderBy(f => f.OrderIndex)
                        .ToList();

                    if (frames.Any())
                    {
                        normalized.StartTime = frames.First().StartTimeSlot;
                        normalized.EndTime = frames.Last().EndTimeSlot;
                    }
                }
            }
            // Handle custom time
            else if (!string.IsNullOrEmpty(parsed.StartTime) && !string.IsNullOrEmpty(parsed.EndTime))
            {
                if (TimeOnly.TryParse(parsed.StartTime, out var start) &&
                    TimeOnly.TryParse(parsed.EndTime, out var end))
                {
                    normalized.StartTime = start;
                    normalized.EndTime = end;
                }
            }

            normalized.StudentCount = parsed.StudentCount > 0 ? parsed.StudentCount : 1;
            normalized.RecurringCount = Math.Min(parsed.RecurringCount, 4); // Max 4 weeks
            normalized.OriginalPrompt = parsed;

            return normalized;
        }

        /// <summary>
        /// Giai đoạn 3: Detect conflicts
        /// </summary>
        private async Task<ConflictDetectionResult> DetectConflictsAsync(
            NormalizedBookingRequest normalized,
            CancellationToken ct)
        {
            var result = new ConflictDetectionResult();
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            if (currentUserId == Guid.Empty || normalized.LabRoomId == 0)
                return result;

            // Check for each week
            for (int i = 0; i < normalized.RecurringCount; i++)
            {
                var weekDate = normalized.BaseDate.AddDays(i * 7);
                var startDateTime = weekDate.Add(normalized.StartTime.ToTimeSpan()).ToUniversalTime();
                var endDateTime = weekDate.Add(normalized.EndTime.ToTimeSpan()).ToUniversalTime();

                // Check personal schedule conflicts
                var scheduleConflicts = await _unitOfWork.Repository<Schedule>().Entities
                    .Where(s => s.LecturerId == currentUserId &&
                                s.IsActive &&
                                !s.IsDeleted &&
                                s.StartTime < endDateTime &&
                                s.EndTime > startDateTime)
                    .ToListAsync(ct);

                if (scheduleConflicts.Any())
                {
                    result.HasScheduleConflict = true;
                    result.ConflictingWeeks.Add(i + 1);
                    result.ConflictDetails.Add($"Tuần {i + 1}: Trùng lịch cá nhân ({scheduleConflicts.Count} sự kiện)");
                }

                // Check other booking requests
                var bookingConflicts = await _unitOfWork.Repository<BookingRequest>().Entities
                    .Include(b => b.Booking)
                    .Where(b => b.RequestedByUserId == currentUserId &&
                                b.BookingRequestStatus == BookingRequestStatus.Pending &&
                                b.Booking.StartTime < endDateTime &&
                                b.Booking.EndTime > startDateTime)
                    .ToListAsync(ct);

                if (bookingConflicts.Any())
                {
                    result.HasBookingConflict = true;
                    result.ConflictDetails.Add($"Tuần {i + 1}: Bạn đã đặt {bookingConflicts.Count} phòng khác");
                }

                // Check room capacity at this time
                var roomBookings = await _unitOfWork.Repository<Schedule>().Entities
                    .CountAsync(s => s.LabRoomId == normalized.LabRoomId &&
                                      s.StartTime < endDateTime &&
                                      s.EndTime > startDateTime, ct);

                if (roomBookings >= 10) // Giả sử max 10 concurrent bookings
                {
                    result.RoomCapacityIssues.Add($"Tuần {i + 1}: Phòng sắp đầy");
                }
            }

            return result;
        }

        /// <summary>
        /// Giai đoạn 5: Generate alternative suggestions
        /// </summary>
        private async Task<List<BookingSuggestion>> GenerateAlternativesAsync(
            NormalizedBookingRequest normalized,
            ConflictDetectionResult conflicts,
            CancellationToken ct)
        {
            var suggestions = new List<BookingSuggestion>();

            if (!conflicts.HasScheduleConflict && !conflicts.HasBookingConflict && conflicts.RoomCapacityIssues.Count == 0)
            {
                // No conflicts - no alternatives needed
                return suggestions;
            }

            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // Strategy 1: Same time, different room
            if (normalized.LabRoomId > 0)
            {
                var alternateRooms = await _unitOfWork.Repository<LabRoom>().Entities
                    .Where(r => r.IsActive && r.Id != normalized.LabRoomId)
                    .Take(3)
                    .ToListAsync(ct);

                foreach (var room in alternateRooms)
                {
                    suggestions.Add(new BookingSuggestion
                    {
                        Title = $"Phòng {room.RoomName}",
                        Description = "Cùng giờ, phòng khác",
                        LabRoomId = room.Id,
                        Date = normalized.BaseDate,
                        StartTime = normalized.StartTime,
                        EndTime = normalized.EndTime,
                        ReasonForSuggestion = "Room swap to avoid conflict",
                        MatchScore = 80
                    });
                }
            }

            // Strategy 3: Different date
            for (int daysOffset = 1; daysOffset <= 7; daysOffset++)
            {
                var altDate = normalized.BaseDate.AddDays(daysOffset);
                suggestions.Add(new BookingSuggestion
                {
                    Title = $"Ngày {altDate:dd/MM/yyyy}",
                    Description = $"{daysOffset} ngày sau",
                    LabRoomId = normalized.LabRoomId,
                    Date = altDate,
                    StartTime = normalized.StartTime,
                    EndTime = normalized.EndTime,
                    ReasonForSuggestion = "Date shift to avoid conflict",
                    MatchScore = 70 - daysOffset
                });
            }

            // Sort by match score and return top 5
            return suggestions.OrderByDescending(s => s.MatchScore).Take(5).ToList();
        }

        /// <summary>
        /// Giai đoạn 6: Build final response with confidence scoring
        /// </summary>
        private AIBookingResponse BuildResponse(
            NormalizedBookingRequest normalized,
            List<BookingSuggestion> alternatives,
            ConflictDetectionResult conflicts)
        {
            var baseConfidence = normalized.OriginalPrompt?.Confidence ?? 50;

            // Adjust confidence based on conflicts
            if (conflicts.HasScheduleConflict || conflicts.HasBookingConflict)
                baseConfidence -= 25;
            if (conflicts.RoomCapacityIssues.Any())
                baseConfidence -= 15;

            var status = AIResponseStatus.Success;
            var message = "OK";

            if (normalized.LabRoomId == 0)
            {
                status = AIResponseStatus.MissingRoom;
                message = "Chưa xác định phòng. Bạn muốn đặt phòng nào?";
            }

            if (conflicts.HasScheduleConflict || conflicts.HasBookingConflict)
            {
                status = AIResponseStatus.ConflictDetected;
                message = "Phát hiện trùng lịch. Dưới đây là các lựa chọn thay thế.";
            }

            return new AIBookingResponse
            {
                Status = status,
                Message = message,
                Confidence = Math.Max(0, baseConfidence),
                PrimaryBooking = normalized,
                Suggestions = alternatives,
                ConflictDetails = conflicts.ConflictDetails,
                RequiresUserConfirmation = conflicts.HasScheduleConflict || conflicts.HasBookingConflict || normalized.LabRoomId == 0
            };
        }

        /// <summary>
        /// Build context data cho Gemini prompt
        /// </summary>
        private async Task<AIContextData> BuildAIContextAsync(CancellationToken ct)
        {
            var campusId = _currentUserService.CampusId;

            var rooms = await _unitOfWork.Repository<LabRoom>().Entities
                .Where(r => r.IsActive)
                .ToListAsync(ct);

            var purposes = await _unitOfWork.Repository<PurposeType>().GetAllAsync();

            var slotTypes = await _unitOfWork.Repository<SlotType>().Entities
                .Include(s => s.SlotFrames)
                .Where(s => s.CampusId == campusId)
                .OrderBy(s => s.Name)
                .ToListAsync(ct);

            var roomContext = string.Join("\n", rooms.Select(r => $"- {r.RoomNo}: {r.RoomName} ({r.Capacity} chỗ)"));

            var purposeContext = string.Join(", ", purposes.Select(p => p.PurposeName));

            var slotContext = string.Join("\n", slotTypes.Select(s =>
                $"- {s.Name}: [{string.Join(", ", s.SlotFrames.OrderBy(f => f.OrderIndex).Select(f => $"{f.OrderIndex}({f.StartTimeSlot:HH\\:mm}-{f.EndTimeSlot:HH\\:mm})"))}]"));

            return new AIContextData
            {
                RoomContext = roomContext,
                PurposeTypesContext = purposeContext,
                SlotTypesContext = slotContext
            };
        }
    }
}