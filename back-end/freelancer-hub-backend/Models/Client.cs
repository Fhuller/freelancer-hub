namespace freelancer_hub_backend.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}