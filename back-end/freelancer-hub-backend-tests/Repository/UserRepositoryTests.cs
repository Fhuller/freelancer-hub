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
    public class UserRepositoryTests
    {
        private FreelancerContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FreelancerContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_User()
        {
            using var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Felipe Costa",
                Email = "felipe@example.com",
                Language = "pt"
            };

            await repository.AddAsync(user);

            var saved = await context.Users.FirstOrDefaultAsync(u => u.Email == "felipe@example.com");
            Assert.NotNull(saved);
            Assert.Equal("Felipe Costa", saved!.Name);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Users_Ordered_By_CreatedAt_Descending()
        {
            using var context = GetInMemoryContext();
            context.Users.AddRange(
                new User { Id = "1", Name = "Antigo", Email = "a@example.com", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new User { Id = "2", Name = "Novo", Email = "b@example.com", CreatedAt = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);
            var result = (await repository.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Novo", result.First().Name);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_User()
        {
            using var context = GetInMemoryContext();
            var id = Guid.NewGuid().ToString();
            context.Users.Add(new User
            {
                Id = id,
                Name = "Teste Usuário",
                Email = "teste@teste.com"
            });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);
            var result = await repository.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Teste Usuário", result!.Name);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_User_Data()
        {
            using var context = GetInMemoryContext();
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Usuário Original",
                Email = "original@example.com"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            user.Name = "Usuário Atualizado";
            var repository = new UserRepository(context);
            await repository.UpdateAsync(user);

            var updated = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.NotNull(updated);
            Assert.Equal("Usuário Atualizado", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_User()
        {
            using var context = GetInMemoryContext();
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Para Deletar",
                Email = "delete@example.com"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);
            await repository.DeleteAsync(user);

            var exists = await context.Users.AnyAsync(u => u.Id == user.Id);
            Assert.False(exists);
        }

        [Fact]
        public async Task EmailExistsAsync_Should_Return_True_When_Email_Exists()
        {
            using var context = GetInMemoryContext();
            context.Users.Add(new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Existente",
                Email = "existe@example.com"
            });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);
            var exists = await repository.EmailExistsAsync("existe@example.com");

            Assert.True(exists);
        }

        [Fact]
        public async Task EmailExistsAsync_Should_Return_False_When_Email_Not_Exists()
        {
            using var context = GetInMemoryContext();
            var repository = new UserRepository(context);

            var exists = await repository.EmailExistsAsync("naoexiste@example.com");

            Assert.False(exists);
        }

        [Fact]
        public async Task EmailExistsAsync_Should_Ignore_Excluded_UserId()
        {
            using var context = GetInMemoryContext();
            var userId = Guid.NewGuid().ToString();

            context.Users.Add(new User
            {
                Id = userId,
                Name = "Usuário Teste",
                Email = "teste@teste.com"
            });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);
            var exists = await repository.EmailExistsAsync("teste@teste.com", excludeUserId: userId);

            Assert.False(exists);
        }

        [Fact]
        public async Task EmailExistsAsync_Should_Return_True_When_Another_User_Has_Same_Email()
        {
            using var context = GetInMemoryContext();
            var user1 = new User { Id = "1", Name = "User 1", Email = "teste@teste.com" };
            var user2 = new User { Id = "2", Name = "User 2", Email = "outro@teste.com" };
            context.Users.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);
            var exists = await repository.EmailExistsAsync("teste@teste.com", excludeUserId: "2");

            Assert.True(exists);
        }
    }
}
