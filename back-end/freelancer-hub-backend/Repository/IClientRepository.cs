using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;

namespace freelancer_hub_backend.Repository
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetClientByUserIdAsync(string userId);
        Task<Client> GetClientByIdAsync(Guid id);
        Task AddClientAsync(Client client);
        Task<bool> EmailExistsAsync(string userId, string email);
    }
}
