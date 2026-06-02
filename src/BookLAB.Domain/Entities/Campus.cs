namespace BookLAB.Domain.Entities
{
    public class Campus
    {
        public int Id { get; set; }
        public string CampusName { get; set; } = null!;
        public string? CampusCode { get; set; }
        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? CampusImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Building> Buildings { get; set; }

    }
}
