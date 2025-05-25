namespace freelancer_hub_backend.DTO_s
{
    public class ExpenseCreateDto
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Category { get; set; } = default!;
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }

    public class ExpenseUpdateDto
    {
        public string Title { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Category { get; set; } = default!;
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Category { get; set; } = default!;
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
