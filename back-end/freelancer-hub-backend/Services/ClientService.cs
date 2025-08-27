using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<ClientReadDto>> GetClientsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var clients = await _clientRepository.GetClientByUserIdAsync(userId);

            return clients.Select(c => new ClientReadDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone,
                CompanyName = c.CompanyName,
                Notes = c.Notes,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<ClientReadDto> GetClientByIdAsync(Guid id)
        {
            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
                return null;

            return new ClientReadDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                CompanyName = client.CompanyName,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task<ClientReadDto> CreateClientAsync(string userId, ClientCreateDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            await ValidateClientAsync(userId, dto);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                CompanyName = dto.CompanyName,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _clientRepository.AddClientAsync(client);

            return new ClientReadDto
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                CompanyName = client.CompanyName,
                Notes = client.Notes,
                CreatedAt = client.CreatedAt
            };
        }

        private async Task ValidateClientAsync(string userId, ClientCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("O nome do cliente é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("O email do cliente é obrigatório.");

            try
            {
                var _ = new System.Net.Mail.MailAddress(dto.Email);
            }
            catch
            {
                throw new ArgumentException("O email fornecido é inválido.");
            }

            bool emailExists = await _clientRepository.EmailExistsAsync(userId, dto.Email);

            if (emailExists)
                throw new ArgumentException("Já existe um cliente com este email.");
        }
    }
}
