namespace freelancer_hub_backend.Models
{
    public class ProjectTag
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid TagId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}