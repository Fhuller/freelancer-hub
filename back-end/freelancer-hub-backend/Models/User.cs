namespace freelancer_hub_backend.Models
{
    public class User
    {
        public required string Id { get; set; }           // UUID do Supabase
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Language { get; set; } = "pt";
    }
}