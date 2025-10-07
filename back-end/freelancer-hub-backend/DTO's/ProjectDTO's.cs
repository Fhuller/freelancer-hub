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
        public decimal HourlyRate { get; set; } // Adicionado
    }

    public class ProjectUpdateDto
    {
        public Guid? ClientId { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public string Status { get; set; } = default!;
        public DateTime? DueDate { get; set; }
        public decimal? HourlyRate { get; set; } // Opcional na atualização geral
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
        public decimal HourlyRate { get; set; }
        public decimal TotalHours { get; set; } // Adicionado
        public decimal TotalEarned => TotalHours * HourlyRate; // Calculado
    }

    // DTO específica para atualização de horas e taxa
    public class UpdateProjectHoursDto
    {
        public decimal? TotalHours { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? HoursToAdd { get; set; } // Para adicionar horas incrementalmente
        public string? Description { get; set; } // Descrição opcional para a alteração
    }

    // DTO para resposta de horas
    public class ProjectHoursSummaryDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectTitle { get; set; } = default!;
        public decimal TotalHours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalEarned { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}