using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly FreelancerContext _context;

        public ClientRepository(FreelancerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetClientByUserIdAsync(string userId)
        {
            return await _context.Clients
            .Where(c => c.UserId == userId)
            .ToListAsync();
        }

        public async Task<Client> GetClientByIdAsync(Guid id)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string userId, string email)
        {
            return await _context.Clients
                .AnyAsync(c => c.UserId == userId && c.Email == email);
        }
    }
}
