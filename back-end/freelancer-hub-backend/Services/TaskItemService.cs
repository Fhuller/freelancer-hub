using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;

namespace freelancer_hub_backend.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public TaskItemService(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;
        }

        public async Task<IEnumerable<TaskItemDto>> GetTaskItemsByProjectAsync(string userId, Guid projectId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            // Verificar se o projeto pertence ao usuário
            bool projectExists = await _taskItemRepository.ProjectExistsForUserAsync(userId, projectId);
            if (!projectExists)
                throw new ArgumentException("Projeto não encontrado ou não pertence ao usuário.");

            var taskItems = await _taskItemRepository.GetByProjectIdAsync(projectId);

            return taskItems.Select(t => new TaskItemDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt
            });
        }

        public async Task<TaskItemDto> GetTaskItemByIdAsync(Guid id)
        {
            var taskItem = await _taskItemRepository.GetByIdWithProjectAsync(id);

            if (taskItem == null)
                return null;

            return new TaskItemDto
            {
                Id = taskItem.Id,
                ProjectId = taskItem.ProjectId,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Status = taskItem.Status,
                DueDate = taskItem.DueDate,
                CreatedAt = taskItem.CreatedAt
            };
        }

        public async Task<TaskItemDto> CreateTaskItemAsync(string userId, TaskItemCreateDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            await ValidateTaskItemAsync(userId, dto);

            var taskItem = new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = dto.ProjectId,
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                DueDate = dto.DueDate ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _taskItemRepository.AddAsync(taskItem);

            return new TaskItemDto
            {
                Id = taskItem.Id,
                ProjectId = taskItem.ProjectId,
                Title = taskItem.Title,
                Description = taskItem.Description,
                Status = taskItem.Status,
                DueDate = taskItem.DueDate,
                CreatedAt = taskItem.CreatedAt
            };
        }

        public async Task UpdateTaskItemAsync(Guid id, TaskItemUpdateDto dto)
        {
            var taskItem = await _taskItemRepository.GetByIdWithProjectAsync(id);

            if (taskItem == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            ValidateTaskItemUpdate(dto);

            taskItem.Title = dto.Title;
            taskItem.Description = dto.Description;
            taskItem.Status = dto.Status;
            taskItem.DueDate = dto.DueDate ?? DateTime.UtcNow;

            await _taskItemRepository.UpdateAsync(taskItem);
        }

        public async Task DeleteTaskItemAsync(Guid id)
        {
            var taskItem = await _taskItemRepository.GetByIdAsync(id);

            if (taskItem == null)
                throw new KeyNotFoundException("Tarefa não encontrada.");

            await _taskItemRepository.DeleteAsync(taskItem);
        }

        private async Task ValidateTaskItemAsync(string userId, TaskItemCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("O título da tarefa é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("O status da tarefa é obrigatório.");

            // Validar se o status é válido
            var validStatuses = new[] { "Pendente", "Em Andamento", "Concluída", "Cancelada" };
            if (!validStatuses.Contains(dto.Status))
                throw new ArgumentException($"Status inválido. Valores aceitos: {string.Join(", ", validStatuses)}");

            // Verificar se o projeto existe e pertence ao usuário
            bool projectExists = await _taskItemRepository.ProjectExistsForUserAsync(userId, dto.ProjectId);
            if (!projectExists)
                throw new ArgumentException("Projeto não encontrado ou não pertence ao usuário.");

            // Validar data de vencimento
            if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.Today)
                throw new ArgumentException("A data de vencimento não pode ser anterior à data atual.");

            // Validar tamanho do título
            if (dto.Title.Length > 200)
                throw new ArgumentException("O título não pode ter mais de 200 caracteres.");

            // Validar tamanho da descrição
            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 1000)
                throw new ArgumentException("A descrição não pode ter mais de 1000 caracteres.");
        }

        private void ValidateTaskItemUpdate(TaskItemUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("O título da tarefa é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Status))
                throw new ArgumentException("O status da tarefa é obrigatório.");

            // Validar se o status é válido
            var validStatuses = new[] { "Pendente", "Em Andamento", "Concluída", "Cancelada" };
            if (!validStatuses.Contains(dto.Status))
                throw new ArgumentException($"Status inválido. Valores aceitos: {string.Join(", ", validStatuses)}");

            // Validar data de vencimento
            if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.Today)
                throw new ArgumentException("A data de vencimento não pode ser anterior à data atual.");

            // Validar tamanho do título
            if (dto.Title.Length > 200)
                throw new ArgumentException("O título não pode ter mais de 200 caracteres.");

            // Validar tamanho da descrição
            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 1000)
                throw new ArgumentException("A descrição não pode ter mais de 1000 caracteres.");
        }
    }
}