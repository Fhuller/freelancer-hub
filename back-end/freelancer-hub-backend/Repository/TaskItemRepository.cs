using freelancer_hub_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Repository
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly FreelancerContext _context;

        public TaskItemRepository(FreelancerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(Guid projectId)
        {
            return await _context.TaskItems
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.DueDate)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<TaskItem> GetByIdAsync(Guid id)
        {
            return await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> GetByIdWithProjectAsync(Guid id)
        {
            return await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddAsync(TaskItem taskItem)
        {
            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            _context.Entry(taskItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem taskItem)
        {
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ProjectExistsForUserAsync(string userId, Guid projectId)
        {
            return await _context.Projects
                .AnyAsync(p => p.Id == projectId && p.UserId == userId);
        }
    }
}