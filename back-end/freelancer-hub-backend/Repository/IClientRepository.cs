using freelancer_hub_backend.Models;

namespace freelancer_hub_backend.Repository
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetByUserIdAsync(string userId);
        Task<Client> GetByIdAsync(Guid id);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(Client client);
        Task<bool> EmailExistsAsync(string userId, string email);
    }
}
