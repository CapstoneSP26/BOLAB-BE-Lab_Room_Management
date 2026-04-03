using BookLAB.Api.Controllers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory;
using BookLAB.Application.Features.IncidentReports.Commands.UpdateReport;
using BookLAB.Application.Features.IncidentReports.Queries.GetReportedReport;
using BookLAB.Application.Features.IncidentReports.Queries.GetReports;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ReportsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ReportsController(IMediator mediator, ILogger<ReportsController> logger, IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet("get-incident-reports")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetReportedReportAsync()
        {
            try
            {
                Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

                GetReportedReportCommand command = new GetReportedReportCommand
                {
                    userId = userId
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Problem("Something is wrong");
            }

        }

        [HttpGet("listreports")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetUnresolvedReports([FromQuery] ReportRequestDto dto)
        {
            try
            {
                Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

                GetReportsQuery query = new GetReportsQuery
                {
                    Q = dto.Q,
                    BuildingId = dto.BuildingId,
                    RoomId = dto.RoomId,
                    ReportType = dto.ReportType,
                    IsResolved = dto.IsResolved,
                    FromDate = dto.FromDate,
                    ToDate = dto.ToDate,
                    Page = dto.Page,
                    Limit = dto.Limit,
                    SortBy = dto.SortBy,
                    IsDescending = dto.IsDescending,
                    UserId = userId
                };

                var result = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Problem("Something is wrong");
            }
        }

// =====================================================================
        // New endpoints to integrate with FE "Send Report"
        // =====================================================================

        public class CreateReportRequest
        {
            public string RoomId { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int? ReportTypeId { get; set; }  // Must be 1-9
            public List<IFormFile>? Images { get; set; }
        }

        public class ResolveReportRequest
        {
            public bool IsResolved { get; set; }
        }

        private static object MapToResponse(Report report, Schedule? schedule, string? username = "unknown", string? reasonOverride = null)
        {
            var labRoom = schedule?.LabRoom;
            var building = labRoom?.Building;

            return new
            {
                Id = report.Id,
                ScheduleId = report.ScheduleId,
                UserId = report.CreatedBy,
                ReportType = report.ReportType?.ReportTypeName ?? "Unknown",
                Description = report.Description,
                IsResolved = report.IsResolved,
                LabRoomId = labRoom?.Id,
                RoomName = labRoom?.RoomName,
                BuildingName = building?.BuildingName,
                Reason = reasonOverride,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt,
                CreatedBy = report.CreatedBy,
                UpdatedBy = report.UpdatedBy,
                UserName = username,
            };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = "Lecturer")]
        public async Task<IActionResult> CreateReportAsync([FromForm] CreateReportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { success = false, message = "User not found in token" });
                }

                if (string.IsNullOrWhiteSpace(request.RoomId) || string.IsNullOrWhiteSpace(request.Description))
                {
                    return BadRequest(new { success = false, message = "roomId and description are required" });
                }

                if (!int.TryParse(request.RoomId, out var roomId))
                {
                    return BadRequest(new { success = false, message = "roomId must be a number" });
                }

                var schedule = await _unitOfWork.Repository<Schedule>().Entities
                    .Include(s => s.LabRoom)
                    .ThenInclude(r => r.Building)
                    .Where(s => s.LabRoomId == roomId)
                    .OrderByDescending(s => s.StartTime)
                    .FirstOrDefaultAsync(cancellationToken);

                if (schedule == null)
                {
                    return NotFound(new { success = false, message = "No schedule found for this room" });
                }

                // Determine ReportTypeId: must be provided by frontend
                if (!request.ReportTypeId.HasValue || request.ReportTypeId <= 0 || request.ReportTypeId > 9)
                {
                    return BadRequest(new { success = false, message = "Valid ReportTypeId (1-9) is required" });
                }

                int reportTypeId = request.ReportTypeId.Value;

                var report = new Report
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedule.Id,
                    ReportTypeId = reportTypeId,
                    Description = request.Description.Trim(),
                    IsResolved = false,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = userId
                };

                await _unitOfWork.Repository<Report>().AddAsync(report);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Image upload handling can be added later. FE accepts empty images list.

                return Ok(new
                {
                    success = true,
                    message = "Report created",
                    data = MapToResponse(report, schedule)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create report");
                return Problem("Failed to create report");
            }
        }

        [HttpGet("reasons")]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public IActionResult GetReportReasons()
        {
            var reasons = new[]
            {
                new { value = "equipment_damaged", label = "Thiết bị hư hỏng" },
                new { value = "equipment_missing", label = "Thiết bị mất" },
                new { value = "cleanliness_issue", label = "Vấn đề vệ sinh" },
                new { value = "other", label = "Khác" }
            };

            return Ok(new { success = true, data = reasons });
        }

        [HttpGet("my-reports")]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetMyReportsAsync(CancellationToken cancellationToken)
        {
            Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

            if (userId == Guid.Empty)
            {
                return Unauthorized(new { success = false, message = "User not found in token" });
            }

            var reports = await _unitOfWork.Repository<Report>().Entities
                .Include(r => r.Schedule)
                .ThenInclude(s => s.LabRoom)
                .ThenInclude(l => l.Building)
                .Include(r => r.ReportType)
                .Where(r => r.CreatedBy == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            var data = reports.Select(r => MapToResponse(r, r.Schedule)).ToList();

            return Ok(new
            {
                success = true,
                data = new
                {
                    reports = data,
                    total = data.Count,
                    page = 1,
                    limit = data.Count,
                    totalPages = 1
                }
            });
        }

        [HttpGet("history")]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetReportHistoryAsync(CancellationToken cancellationToken)
        {
            var reports = await _unitOfWork.Repository<Report>().Entities
                .Include(r => r.Schedule)
                .ThenInclude(s => s.LabRoom)
                .ThenInclude(l => l.Building)
                .Include(r => r.ReportType)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            var data = reports.Select(r => MapToResponse(r, r.Schedule)).ToList();

            return Ok(new { data });
        }

        [HttpGet("~/api/listreports")]
        [Authorize(Policy = "Lecturer")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetReportListAsync(CancellationToken cancellationToken)
        {
            var reports = await _unitOfWork.Repository<Report>().Entities
                .Include(r => r.Schedule)
                .ThenInclude(s => s.LabRoom)
                .ThenInclude(l => l.Building)
                .Include(r => r.ReportType)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            var data = reports.Select(r => MapToResponse(r, r.Schedule)).ToList();

            return Ok(new { data });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> GetReportDetailAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.Repository<Report>().Entities
                .Include(r => r.Schedule)
                .ThenInclude(s => s.LabRoom)
                .ThenInclude(l => l.Building)
                .Include(r => r.ReportType)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (report == null)
            {
                return NotFound(new { success = false, message = "Report not found" });
            }

            // Load images if available
            var images = await _unitOfWork.Repository<ReportImage>().Entities
                .Where(img => img.ReportId == report.Id)
                .ToListAsync(cancellationToken);

            var mappedImages = images.Select(img => new
            {
                Id = img.Id,
                ReportId = img.ReportId,
                ImageLink = img.ImageUrl,
                Size = img.Size,
                FileType = img.FileType.ToString()
            });

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(report.CreatedBy);

            return Ok(new
            {
                success = true,
                data = MapToResponse(report, report.Schedule, user.FullName),
                images = mappedImages
            });
        }

        [HttpPatch("{id}/resolve")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> ResolveReportAsync([FromRoute] Guid id, [FromBody] ResolveReportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

                var report = await _unitOfWork.Repository<Report>().Entities
                    .Include(r => r.Schedule)
                    .ThenInclude(s => s.LabRoom)
                    .ThenInclude(l => l.Building)
                    .Include(r => r.ReportType)
                    .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

                if (report == null)
                {
                    return NotFound(new { success = false, message = "Report not found" });
                }

                report.IsResolved = request.IsResolved;
                report.UpdatedAt = DateTimeOffset.UtcNow;
                report.UpdatedBy = userId == Guid.Empty ? report.UpdatedBy : userId;

                _unitOfWork.Repository<Report>().Update(report);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Ok(new { data = MapToResponse(report, report.Schedule) });
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        [HttpPost("resolved")]
        [Authorize(Policy = "AcademicOffice_LabManager")]
        public async Task<IActionResult> ResolveReport([FromQuery] Guid reportId, [FromBody] TempReport tempReport)
        {
            try
            {
                Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out var userId);

                UpdateReportCommand command = new UpdateReportCommand
                {
                    ReportId = reportId,
                    TempReport = tempReport
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    success = result
                });
            }
            catch (Exception ex)
            {
                return Problem("Something is wrong");
            }

        }
    }
}


