using freelancer_hub_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly FreelancerContext _context;

        public UserRepository(FreelancerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, string excludeUserId = null)
        {
            var query = _context.Users.Where(u => u.Email == email);

            if (!string.IsNullOrEmpty(excludeUserId))
            {
                query = query.Where(u => u.Id != excludeUserId);
            }

            return await query.AnyAsync();
        }
    }
}