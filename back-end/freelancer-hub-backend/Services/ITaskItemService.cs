using freelancer_hub_backend.DTO_s;

namespace freelancer_hub_backend.Services
{
    public interface ITaskItemService
    {
        Task<IEnumerable<TaskItemDto>> GetTaskItemsByProjectAsync(string userId, Guid projectId);
        Task<TaskItemDto> GetTaskItemByIdAsync(Guid id);
        Task<TaskItemDto> CreateTaskItemAsync(string userId, TaskItemCreateDto dto);
        Task UpdateTaskItemAsync(Guid id, TaskItemUpdateDto dto);
        Task DeleteTaskItemAsync(Guid id);
    }
}