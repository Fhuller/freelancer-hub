namespace freelancer_hub_backend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Client> Clients { get; set; } = new List<Client>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }

}
