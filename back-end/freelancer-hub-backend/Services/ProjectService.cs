using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;

namespace freelancer_hub_backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var projects = await _projectRepository.GetByUserIdAsync(userId);

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                UserId = p.UserId,
                ClientId = p.ClientId,
                Title = p.Title,
                Description = p.Description,
                Status = p.Status,
                DueDate = p.DueDate,
                CreatedAt = p.CreatedAt
            });
        }

        public async Task<ProjectDto> GetProjectByIdAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdWithClientAsync(id);

            if (project == null)
                return null;

            return new ProjectDto
            {
                Id = project.Id,
                UserId = project.UserId,
                ClientId = project.ClientId,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                DueDate = project.DueDate,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task<ProjectDto> CreateProjectAsync(string userId, ProjectCreateDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            await ValidateProjectAsync(userId, dto);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ClientId = (Guid)dto.ClientId,
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                DueDate = dto.DueDate ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(project);

            return new ProjectDto
            {
                Id = project.Id,
                UserId = project.UserId,
                ClientId = project.ClientId,
                Title = project.Title,
                Description = project.Description,
                Status = project.Status,
                DueDate = project.DueDate,
                CreatedAt = project.CreatedAt
            };
        }

        public async Task UpdateProjectAsync(Guid id, ProjectUpdateDto dto)
        {
            var project = await _projectRepository.GetByIdAsync(id);

            if (project == null)
                throw new KeyNotFoundException("Projeto não encontrado.");

            ValidateProjectUpdate(dto);

            if (dto.ClientId != project.ClientId)
            {
                bool clientExists = await _projectRepository.ClientExistsForUserAsync(project.UserId, dto.ClientId);
                if (!clientExists)
                    throw new ArgumentException("Cliente não encontrado ou não pertence ao usuário.");
            }

            project.Title = dto.Title;
            project.Description = dto.Description;
            project.Status = dto.Status;
            project.DueDate = dto.DueDate ?? project.DueDate;
            project.ClientId = dto.ClientId ?? project.ClientId;

            await _projectRepository.UpdateAsync(project);
        }

        public async Task DeleteProjectAsync(Guid id)
        {
            var project = await _projectRepository.GetByIdAsync(id);

            if (project == null)
                throw new KeyNotFoundException("Projeto não encontrado.");

            await _projectRepository.DeleteAsync(project);
        }

        private async Task ValidateProjectAsync(string userId, ProjectCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("O título do projeto é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("O status do projeto é obrigatório.");

            var validStatuses = new[] { "Pendente", "Em Andamento", "Concluído", "Cancelado" };
            if (!validStatuses.Contains(dto.Status))
                throw new ArgumentException($"Status inválido. Valores aceitos: {string.Join(", ", validStatuses)}");

            if (dto.ClientId != null)
            {
                bool clientExists = await _projectRepository.ClientExistsForUserAsync(userId, dto.ClientId);
                if (!clientExists)
                    throw new ArgumentException("Cliente não encontrado ou não pertence ao usuário.");
            }
            else
                throw new ArgumentException("Cliente não pode ser nulo");

            if (dto.DueDate != null && dto.DueDate.Value.Date < DateTime.Today)
                throw new ArgumentException("A data de vencimento não pode ser anterior à data atual.");
        }

        private void ValidateProjectUpdate(ProjectUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("O título do projeto é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("O status do projeto é obrigatório.");

            var validStatuses = new[] { "Pendente", "Em Andamento", "Concluído", "Cancelado" };
            if (!validStatuses.Contains(dto.Status))
                throw new ArgumentException($"Status inválido. Valores aceitos: {string.Join(", ", validStatuses)}");

            if (dto.DueDate != null && dto.DueDate.Value.Date < DateTime.Today)
                throw new ArgumentException("A data de vencimento não pode ser anterior à data atual.");
        }
    }
}