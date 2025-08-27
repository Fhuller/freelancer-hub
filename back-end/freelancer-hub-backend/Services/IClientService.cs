using freelancer_hub_backend.DTO_s;

namespace freelancer_hub_backend.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientReadDto>> GetClientsAsync(string userId);
        Task<ClientReadDto> GetClientByIdAsync(Guid id);
        Task<ClientReadDto> CreateClientAsync(string userId, ClientCreateDto dto);
    }
}
