namespace BookLAB.Application.Features.Profile.DTOs;

public class RecentActivityDto
{
    /// <summary>
    /// ID hoạt động (có thể null)
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Loại hoạt động: Booking, Schedule, Attendance, Report, etc.
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Tiêu đề hoạt động
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả chi tiết
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Phòng lab liên quan
    /// </summary>
    public string? LabRoomName { get; set; }

    /// <summary>
    /// Ngày thực hiện
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public string? Status { get; set; }
}
