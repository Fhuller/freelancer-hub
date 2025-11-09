using System;
using System.Linq;
using System.Threading.Tasks;
using freelancer_hub_backend;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace freelancer_hub_backend_tests.Repository
{
    public class ExpenseRepositoryTests
    {
        private FreelancerContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FreelancerContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Expense()
        {
            using var context = GetInMemoryContext();
            var repository = new ExpenseRepository(context);

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                Title = "Assinatura Adobe",
                Amount = 59.90m,
                Category = "Software",
                PaymentDate = DateTime.UtcNow,
                Notes = "Pagamento mensal"
            };

            await repository.AddAsync(expense);

            var saved = await context.Expenses.FirstOrDefaultAsync(e => e.Title == "Assinatura Adobe");
            Assert.NotNull(saved);
            Assert.Equal("user1", saved!.UserId);
            Assert.Equal(59.90m, saved.Amount);
        }

        [Fact]
        public async Task GetByUserIdAsync_Should_Return_Only_User_Expenses_In_Descending_Order()
        {
            using var context = GetInMemoryContext();
            context.Expenses.AddRange(
                new Expense { Id = Guid.NewGuid(), UserId = "u1", Title = "Internet", Amount = 100, Category = "Utilidades", PaymentDate = DateTime.UtcNow.AddDays(-1) },
                new Expense { Id = Guid.NewGuid(), UserId = "u1", Title = "Energia", Amount = 150, Category = "Utilidades", PaymentDate = DateTime.UtcNow },
                new Expense { Id = Guid.NewGuid(), UserId = "u2", Title = "Streaming", Amount = 30, Category = "Lazer", PaymentDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var repository = new ExpenseRepository(context);
            var result = (await repository.GetByUserIdAsync("u1")).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Energia", result.First().Title); // deve vir primeiro (ordem decrescente por PaymentDate)
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Expense()
        {
            using var context = GetInMemoryContext();
            var id = Guid.NewGuid();
            var expense = new Expense
            {
                Id = id,
                UserId = "u1",
                Title = "Compra de plugin",
                Amount = 20.50m,
                Category = "Software",
                PaymentDate = DateTime.UtcNow
            };
            context.Expenses.Add(expense);
            await context.SaveChangesAsync();

            var repository = new ExpenseRepository(context);
            var result = await repository.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Compra de plugin", result!.Title);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Expense()
        {
            using var context = GetInMemoryContext();
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                Title = "Spotify",
                Amount = 19.90m,
                Category = "Lazer",
                PaymentDate = DateTime.UtcNow
            };
            context.Expenses.Add(expense);
            await context.SaveChangesAsync();

            expense.Amount = 24.90m;
            var repository = new ExpenseRepository(context);
            await repository.UpdateAsync(expense);

            var updated = await context.Expenses.FirstOrDefaultAsync(e => e.Id == expense.Id);
            Assert.NotNull(updated);
            Assert.Equal(24.90m, updated!.Amount);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Expense()
        {
            using var context = GetInMemoryContext();
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                Title = "Domínio",
                Amount = 50.00m,
                Category = "Infraestrutura",
                PaymentDate = DateTime.UtcNow
            };
            context.Expenses.Add(expense);
            await context.SaveChangesAsync();

            var repository = new ExpenseRepository(context);
            await repository.DeleteAsync(expense);

            var exists = await context.Expenses.AnyAsync(e => e.Id == expense.Id);
            Assert.False(exists);
        }
    }
}
