using freelancer_hub_backend.Models;

namespace freelancer_hub_backend.Repository
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<Expense>> GetByUserIdAsync(string userId);
        Task<Expense> GetByIdAsync(Guid id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
    }
}