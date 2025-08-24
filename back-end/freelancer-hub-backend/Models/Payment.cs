namespace freelancer_hub_backend.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}