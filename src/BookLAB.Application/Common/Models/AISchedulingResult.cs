using System.Text.Json.Serialization;

namespace BookLAB.Application.Common.Models
{
    public enum AIResponseStatus
    {
        Success,
        MissingRoom,
        ConflictDetected,
        ParseError,
        SystemError
    }

    public class AIBookingResponse
    {
        public AIResponseStatus Status { get; set; }
        public string Message { get; set; } = "";
        public double Confidence { get; set; } // 0-100
        public NormalizedBookingRequest? PrimaryBooking { get; set; }
        public List<BookingSuggestion> Suggestions { get; set; } = new();
        public List<string> ConflictDetails { get; set; } = new();
        public bool RequiresUserConfirmation { get; set; }
    }

    public class NormalizedBookingRequest
    {
        public int LabRoomId { get; set; }
        public string? RoomName { get; set; }
        public int PurposeTypeId { get; set; }
        public int? SlotTypeId { get; set; }
        public List<int> SlotIndices { get; set; } = new();
        public DateTime BaseDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int StudentCount { get; set; }
        public int RecurringCount { get; set; }
        public string? OriginalPromptText { get; set; }
        public AISchedulingResult? OriginalPrompt { get; set; }
    }

    public class BookingSuggestion
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int LabRoomId { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ReasonForSuggestion { get; set; } = "";
        public int MatchScore { get; set; } // 0-100, higher = better
    }

    public class ConflictDetectionResult
    {
        public bool HasScheduleConflict { get; set; }
        public bool HasBookingConflict { get; set; }
        public List<int> ConflictingWeeks { get; set; } = new();
        public List<string> ConflictDetails { get; set; } = new();
        public List<string> RoomCapacityIssues { get; set; } = new();
    }

    public class UserBookingPreferences
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = "";

        [JsonPropertyName("favoriteRooms")]
        public List<int> FavoriteRooms { get; set; } = new();

        [JsonPropertyName("favoritePurposes")]
        public List<int> FavoritePurposes { get; set; } = new();

        [JsonPropertyName("preferredTimes")]
        public List<TimeOnly> PreferredTimes { get; set; } = new();

        [JsonPropertyName("commonPatterns")]
        public List<string> CommonPatterns { get; set; } = new();

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; }
    }

    public class AIContextData
    {
        public string RoomContext { get; set; } = "";
        public string PurposeTypesContext { get; set; } = "";
        public string SlotTypesContext { get; set; } = "";
    }

    public class AISchedulingResult
    {
        [JsonPropertyName("actionType")]
        public string ActionType { get; set; } = "CreateBooking";

        [JsonPropertyName("roomCode")]
        public string? RoomCode { get; set; }

        [JsonPropertyName("date")]
        public string? Date { get; set; }

        [JsonPropertyName("slotTypeName")]
        public string? SlotTypeName { get; set; }

        [JsonPropertyName("slots")]
        public List<int>? Slots { get; set; }

        [JsonPropertyName("startTime")]
        public string? StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public string? EndTime { get; set; }

        [JsonPropertyName("studentCount")]
        public int StudentCount { get; set; } = 1;

        [JsonPropertyName("purposeType")]
        public string? PurposeType { get; set; }

        [JsonPropertyName("recurringCount")]
        public int RecurringCount { get; set; } = 1;

        [JsonPropertyName("confidence")]
        public int Confidence { get; set; }

        [JsonPropertyName("issues")]
        public List<string> Issues { get; set; } = new();
    }

    public enum SuggestionValidationStatus
    {
        Pending,      // Chưa validate
        Validating,   // Đang validate
        Valid,        // ✅ Hợp lệ
        Invalid,      // ❌ Không hợp lệ
        Error         // ⚠️ Lỗi khi validate
    }

    public class SuggestionValidationResult
    {
        public Guid SuggestionId { get; set; }
        public bool IsValid { get; set; }
        public List<ValidationIssue> Issues { get; set; } = new();
        public int Confidence { get; set; } // 0-100, updated after validation
    }

    public class ValidationIssue
    {
        public string Code { get; set; } = "";
        public string Message { get; set; } = "";
        public ValidationIssueSeverity Severity { get; set; }
    }

    public enum ValidationIssueSeverity
    {
        Warning,    // Có thể vượt qua
        Critical    // Không thể đặt được
    }

    public class ParseAndSuggestRequest
    {
        public string UserPrompt { get; set; } = string.Empty;
    }
}