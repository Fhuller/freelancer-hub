namespace freelancer_hub_backend.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public Guid ClientId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Status { get; set; } = "pendente";
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal HourlyRate { get; set; } = 0;
        public decimal TotalHours { get; set; } = 0;
    }
}