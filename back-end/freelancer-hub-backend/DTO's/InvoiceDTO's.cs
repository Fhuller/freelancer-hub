namespace freelancer_hub_backend.DTO_s
{
    public class InvoiceCreateDto
    {
        public required string UserId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public required string Status { get; set; }
        public string? PdfUrl { get; set; }
    }

    public class InvoiceUpdateDto
    {
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public required string Status { get; set; }
        public string? PdfUrl { get; set; }
    }

    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public required string Status { get; set; }
        public string? PdfUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
