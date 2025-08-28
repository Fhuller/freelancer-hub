using freelancer_hub_backend.DTO_s;

namespace freelancer_hub_backend.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDto>> GetExpensesAsync(string userId);
        Task<ExpenseDto> GetExpenseByIdAsync(Guid id);
        Task<ExpenseDto> CreateExpenseAsync(string userId, ExpenseCreateDto dto);
        Task UpdateExpenseAsync(Guid id, ExpenseUpdateDto dto);
        Task DeleteExpenseAsync(Guid id);
    }
}