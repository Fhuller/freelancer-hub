namespace freelancer_hub_backend.DTO_s
{
    public class ProjectCreateDto
    {
        public required string UserId { get; set; }
        public Guid? ClientId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Status { get; set; } = "pendente";
        public DateTime? DueDate { get; set; }
    }

    public class ProjectUpdateDto
    {
        public Guid? ClientId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Status { get; set; } = default!;
        public DateTime? DueDate { get; set; }
    }

    public class ProjectDto
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public Guid ClientId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Status { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
