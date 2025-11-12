using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;
using freelancer_hub_backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace freelancer_hub_backend_tests.Services
{
    public class ExpenseServiceTests
    {
        private readonly Mock<IExpenseRepository> _repositoryMock;
        private readonly ExpenseService _service;

        public ExpenseServiceTests()
        {
            _repositoryMock = new Mock<IExpenseRepository>();
            _service = new ExpenseService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetExpensesAsync_ShouldReturnExpenses_WhenUserIdIsValid()
        {
            var userId = "user1";
            var expenses = new List<Expense>
            {
                new Expense
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Compra de equipamentos",
                    Amount = 500,
                    Category = "Trabalho",
                    PaymentDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow
                }
            };

            _repositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(expenses);

            var result = await _service.GetExpensesAsync(userId);

            Assert.Single(result);
            Assert.Equal("Compra de equipamentos", result.First().Title);
            Assert.Equal(500, result.First().Amount);
        }

        [Fact]
        public async Task GetExpensesAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.GetExpensesAsync(string.Empty));
        }

        [Fact]
        public async Task GetExpenseByIdAsync_ShouldReturnExpense_WhenFound()
        {
            var id = Guid.NewGuid();
            var expense = new Expense
            {
                Id = id,
                UserId = "user1",
                Title = "Mensalidade do coworking",
                Amount = 250,
                Category = "Serviços",
                PaymentDate = DateTime.UtcNow.AddDays(-2)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expense);

            var result = await _service.GetExpenseByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Mensalidade do coworking", result.Title);
            Assert.Equal(250, result.Amount);
        }

        [Fact]
        public async Task GetExpenseByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Expense)null);

            var result = await _service.GetExpenseByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateExpenseAsync_ShouldCreate_WhenValid()
        {
            var userId = "user1";
            var dto = new ExpenseCreateDto
            {
                Title = "Assinatura do GitHub",
                UserId = "user1",
                Amount = 10,
                Category = "Serviços",
                PaymentDate = DateTime.UtcNow.AddDays(-1),
                Notes = "Plano Pro"
            };

            Expense addedExpense = null;

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Expense>()))
                .Callback<Expense>(e => addedExpense = e)
                .Returns(Task.CompletedTask);

            var result = await _service.CreateExpenseAsync(userId, dto);

            Assert.NotNull(result);
            Assert.Equal("Assinatura do GitHub", result.Title);
            Assert.Equal(10, result.Amount);
            Assert.Equal("Serviços", result.Category);
            Assert.Equal(userId, addedExpense.UserId);
            Assert.NotEqual(default, addedExpense.CreatedAt);
        }

        [Fact]
        public async Task CreateExpenseAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            var dto = new ExpenseCreateDto
            {
                Title = "Teste",
                UserId = "user1",
                Amount = 100,
                Category = "Outros",
                PaymentDate = DateTime.UtcNow.AddDays(-1)
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateExpenseAsync(string.Empty, dto));
        }

        [Theory]
        [InlineData("", 100, "Categoria", -1, "O título da despesa é obrigatório.")]
        [InlineData("Compra", 0, "Categoria", -1, "O valor da despesa deve ser maior que zero.")]
        [InlineData("Compra", 50, "", -1, "A categoria da despesa é obrigatória.")]
        [InlineData("Compra", 50, "Categoria", 1, "A data de pagamento não pode ser futura.")]
        public async Task CreateExpenseAsync_ShouldThrow_OnInvalidFields(string title, decimal amount, string category, int daysFromNow, string expectedMessage)
        {
            var dto = new ExpenseCreateDto
            {
                Title = title,
                UserId = "user1",
                Amount = amount,
                Category = category,
                PaymentDate = DateTime.UtcNow.AddDays(daysFromNow)
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateExpenseAsync("user1", dto));

            Assert.Contains(expectedMessage, ex.Message);
        }

        [Fact]
        public async Task UpdateExpenseAsync_ShouldUpdate_WhenValid()
        {
            var id = Guid.NewGuid();
            var existing = new Expense
            {
                Id = id,
                UserId = "user1",
                Title = "Despesa antiga",
                Amount = 50,
                Category = "Outros",
                PaymentDate = DateTime.UtcNow.AddDays(-2)
            };

            var dto = new ExpenseUpdateDto
            {
                Title = "Despesa atualizada",
                Amount = 100,
                Category = "Negócios",
                PaymentDate = DateTime.UtcNow.AddDays(-1),
                Notes = "Atualizado"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Expense>())).Returns(Task.CompletedTask);

            await _service.UpdateExpenseAsync(id, dto);

            Assert.Equal("Despesa atualizada", existing.Title);
            Assert.Equal(100, existing.Amount);
            Assert.Equal("Negócios", existing.Category);
            _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateExpenseAsync_ShouldThrow_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Expense)null);

            var dto = new ExpenseUpdateDto
            {
                Title = "Teste",
                Amount = 50,
                Category = "Outros",
                PaymentDate = DateTime.UtcNow.AddDays(-1)
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateExpenseAsync(Guid.NewGuid(), dto));
        }

        [Theory]
        [InlineData("", 10, "Categoria", -1, "O título da despesa é obrigatório.")]
        [InlineData("Compra", 0, "Categoria", -1, "O valor da despesa deve ser maior que zero.")]
        [InlineData("Compra", 10, "", -1, "A categoria da despesa é obrigatória.")]
        [InlineData("Compra", 10, "Categoria", 2, "A data de pagamento não pode ser futura.")]
        public async Task UpdateExpenseAsync_ShouldThrow_OnInvalidData(string title, decimal amount, string category, int daysFromNow, string expectedMessage)
        {
            var id = Guid.NewGuid();
            var expense = new Expense
            {
                Id = id,
                UserId = "user1",
                Title = "Antiga",
                Amount = 100,
                Category = "Geral",
                PaymentDate = DateTime.UtcNow.AddDays(-2)
            };

            var dto = new ExpenseUpdateDto
            {
                Title = title,
                Amount = amount,
                Category = category,
                PaymentDate = DateTime.UtcNow.AddDays(daysFromNow)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(expense);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateExpenseAsync(id, dto));

            Assert.Contains(expectedMessage, ex.Message);
        }

        [Fact]
        public async Task DeleteExpenseAsync_ShouldDelete_WhenExists()
        {
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                Title = "Assinatura",
                Amount = 15,
                Category = "Serviço",
                PaymentDate = DateTime.UtcNow.AddDays(-2)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(expense.Id)).ReturnsAsync(expense);

            await _service.DeleteExpenseAsync(expense.Id);

            _repositoryMock.Verify(r => r.DeleteAsync(expense), Times.Once);
        }

        [Fact]
        public async Task DeleteExpenseAsync_ShouldThrow_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Expense)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteExpenseAsync(Guid.NewGuid()));
        }
    }
}
