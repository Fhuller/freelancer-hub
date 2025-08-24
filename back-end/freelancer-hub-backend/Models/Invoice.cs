namespace freelancer_hub_backend.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "pendente";
        public string? PdfUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}