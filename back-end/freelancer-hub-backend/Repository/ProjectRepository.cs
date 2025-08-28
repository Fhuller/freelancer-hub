using freelancer_hub_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly FreelancerContext _context;

        public ProjectRepository(FreelancerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Project>> GetByUserIdAsync(string userId)
        {
            return await _context.Projects
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Project> GetByIdAsync(Guid id)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project> GetByIdWithClientAsync(Guid id)
        {
            return await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            _context.Entry(project).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ClientExistsForUserAsync(string userId, Guid? clientId)
        {
            if (clientId == null) return true;

            return await _context.Clients
                .AnyAsync(c => c.Id == clientId && c.UserId == userId);
        }
    }
}