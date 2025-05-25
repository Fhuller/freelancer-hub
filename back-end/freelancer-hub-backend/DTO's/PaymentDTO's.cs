namespace freelancer_hub_backend.DTO_s
{
    public class PaymentCreateDto
    {
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string? Notes { get; set; }
    }

    public class PaymentUpdateDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string? Notes { get; set; }
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
