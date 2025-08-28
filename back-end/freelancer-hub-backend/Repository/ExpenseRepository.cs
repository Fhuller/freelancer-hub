using freelancer_hub_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Repository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly FreelancerContext _context;

        public ExpenseRepository(FreelancerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetByUserIdAsync(string userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.PaymentDate)
                .ToListAsync();
        }

        public async Task<Expense> GetByIdAsync(Guid id)
        {
            return await _context.Expenses
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Expense expense)
        {
            _context.Entry(expense).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Expense expense)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }
}