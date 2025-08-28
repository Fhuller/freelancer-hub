using freelancer_hub_backend.Models;

namespace freelancer_hub_backend.Repository
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetByUserIdAsync(string userId);
        Task<Project> GetByIdAsync(Guid id);
        Task<Project> GetByIdWithClientAsync(Guid id);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(Project project);
        Task<bool> ClientExistsForUserAsync(string userId, Guid? clientId);
    }
}