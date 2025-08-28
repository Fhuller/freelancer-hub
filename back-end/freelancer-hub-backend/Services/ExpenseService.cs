using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;

namespace freelancer_hub_backend.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<IEnumerable<ExpenseDto>> GetExpensesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            var expenses = await _expenseRepository.GetByUserIdAsync(userId);

            return expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                UserId = e.UserId,
                Title = e.Title,
                Amount = e.Amount,
                Category = e.Category,
                PaymentDate = e.PaymentDate,
                Notes = e.Notes,
                CreatedAt = e.CreatedAt
            });
        }

        public async Task<ExpenseDto> GetExpenseByIdAsync(Guid id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);

            if (expense == null)
                return null;

            return new ExpenseDto
            {
                Id = expense.Id,
                UserId = expense.UserId,
                Title = expense.Title,
                Amount = expense.Amount,
                Category = expense.Category,
                PaymentDate = expense.PaymentDate,
                Notes = expense.Notes,
                CreatedAt = expense.CreatedAt
            };
        }

        public async Task<ExpenseDto> CreateExpenseAsync(string userId, ExpenseCreateDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            ValidateExpense(dto);

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = dto.Title,
                Amount = dto.Amount,
                Category = dto.Category,
                PaymentDate = dto.PaymentDate,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _expenseRepository.AddAsync(expense);

            return new ExpenseDto
            {
                Id = expense.Id,
                UserId = expense.UserId,
                Title = expense.Title,
                Amount = expense.Amount,
                Category = expense.Category,
                PaymentDate = expense.PaymentDate,
                Notes = expense.Notes,
                CreatedAt = expense.CreatedAt
            };
        }

        public async Task UpdateExpenseAsync(Guid id, ExpenseUpdateDto dto)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);

            if (expense == null)
                throw new KeyNotFoundException("Despesa não encontrada.");

            ValidateExpenseUpdate(dto);

            expense.Title = dto.Title;
            expense.Amount = dto.Amount;
            expense.Category = dto.Category;
            expense.PaymentDate = dto.PaymentDate;
            expense.Notes = dto.Notes;

            await _expenseRepository.UpdateAsync(expense);
        }

        public async Task DeleteExpenseAsync(Guid id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);

            if (expense == null)
                throw new KeyNotFoundException("Despesa não encontrada.");

            await _expenseRepository.DeleteAsync(expense);
        }

        private void ValidateExpense(ExpenseCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("O título da despesa é obrigatório.");

            if (dto.Amount <= 0)
                throw new ArgumentException("O valor da despesa deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("A categoria da despesa é obrigatória.");

            if (dto.PaymentDate > DateTime.Now)
                throw new ArgumentException("A data de pagamento não pode ser futura.");
        }

        private void ValidateExpenseUpdate(ExpenseUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("O título da despesa é obrigatório.");

            if (dto.Amount <= 0)
                throw new ArgumentException("O valor da despesa deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("A categoria da despesa é obrigatória.");

            if (dto.PaymentDate > DateTime.Now)
                throw new ArgumentException("A data de pagamento não pode ser futura.");
        }
    }
}