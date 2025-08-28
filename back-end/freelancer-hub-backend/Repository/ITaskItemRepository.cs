using freelancer_hub_backend.Models;

namespace freelancer_hub_backend.Repository
{
    public interface ITaskItemRepository
    {
        Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId);
        Task<TaskItem> GetByIdAsync(Guid id);
        Task<TaskItem> GetByIdWithProjectAsync(Guid id);
        Task AddAsync(TaskItem taskItem);
        Task UpdateAsync(TaskItem taskItem);
        Task DeleteAsync(TaskItem taskItem);
        Task<bool> ProjectExistsForUserAsync(string userId, Guid projectId);
    }
}