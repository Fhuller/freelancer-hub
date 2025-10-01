namespace freelancer_hub_backend.Models
{
    public class File
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string FileExtension { get; set; }
        public required string FileUrl { get; set; }
        public decimal FileSize { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
