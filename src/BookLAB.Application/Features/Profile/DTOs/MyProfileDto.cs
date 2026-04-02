namespace BookLAB.Application.Features.Profile.DTOs;

public class MyProfileDto
{
    public Guid Id { get; set; }
    
    public string FullName { get; set; } = default!;
    
    public string Email { get; set; } = default!;
    
    public string UserCode { get; set; } = default!;
    
    public string UserImageUrl { get; set; } = string.Empty;
    
    public string? Role { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    public int CampusId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool? IsActive { get; set; }
}
