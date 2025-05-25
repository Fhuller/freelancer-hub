namespace freelancer_hub_backend.DTO_s
{
    public class ClientCreateDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? Notes { get; set; }
    }

    public class ClientUpdateDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? Notes { get; set; }
    }

    public class ClientReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
