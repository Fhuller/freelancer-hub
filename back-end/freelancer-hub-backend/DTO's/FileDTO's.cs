namespace freelancer_hub_backend.DTO_s
{
    public class FileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public decimal FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
