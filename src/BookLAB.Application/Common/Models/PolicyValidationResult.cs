namespace BookLAB.Application.Common.Models
{
    public record PolicyValidationResult(bool IsSuccess, string? Message = null);
}
