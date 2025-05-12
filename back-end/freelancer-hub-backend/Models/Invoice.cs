namespace freelancer_hub_backend.Models
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ClientId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "pendente";
        public string? PdfUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = default!;
        public Client Client { get; set; } = default!;
        public Project Project { get; set; } = default!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

}
