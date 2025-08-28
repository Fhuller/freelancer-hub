using freelancer_hub_backend.Models;

namespace freelancer_hub_backend.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(string id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> EmailExistsAsync(string email, string excludeUserId = null);
    }
}