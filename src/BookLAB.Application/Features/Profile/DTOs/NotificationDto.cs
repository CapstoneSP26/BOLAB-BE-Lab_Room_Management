namespace BookLAB.Application.Features.Profile.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    
    public Guid? UserId { get; set; }
    
    public string Title { get; set; } = default!;
    
    public string Message { get; set; } = default!;
    
    public string Type { get; set; } = default!;
    
    public bool IsRead { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? ReadAt { get; set; }
}
