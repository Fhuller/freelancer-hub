namespace freelancer_hub_backend.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Color { get; set; } = "#000000";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}