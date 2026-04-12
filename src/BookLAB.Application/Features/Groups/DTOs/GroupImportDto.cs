namespace BookLAB.Application.Features.Groups.DTOs
{
    public class GroupImportDto
    {
        public string GroupName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public bool IsUpdated { get; set; } = false;
        public bool IsValid { get; set; } = true;
    }
}
