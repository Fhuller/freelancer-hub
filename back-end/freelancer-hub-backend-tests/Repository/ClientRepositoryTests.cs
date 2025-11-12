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
    public class ClientRepositoryTests
    {
        private FreelancerContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FreelancerContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Client()
        {
            using var context = GetInMemoryContext();
            var repository = new ClientRepository(context);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                Name = "Felipe",
                Email = "felipe@test.com",
                Phone = "99999-9999",
                CompanyName = "FreelaHub",
                Notes = "Cliente recorrente"
            };

            await repository.AddAsync(client);

            var saved = await context.Clients.FirstOrDefaultAsync(c => c.Email == "felipe@test.com");
            Assert.NotNull(saved);
            Assert.Equal("Felipe", saved!.Name);
            Assert.Equal("user1", saved.UserId);
        }

        [Fact]
        public async Task GetByUserIdAsync_Should_Return_Only_User_Clients()
        {
            using var context = GetInMemoryContext();
            context.Clients.AddRange(
                new Client { Id = Guid.NewGuid(), UserId = "u1", Name = "Cliente 1", Email = "c1@mail.com" },
                new Client { Id = Guid.NewGuid(), UserId = "u2", Name = "Cliente 2", Email = "c2@mail.com" }
            );
            await context.SaveChangesAsync();

            var repository = new ClientRepository(context);
            var result = await repository.GetByUserIdAsync("u1");

            Assert.Single(result);
            Assert.Equal("Cliente 1", result.First().Name);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Client()
        {
            using var context = GetInMemoryContext();
            var id = Guid.NewGuid();
            var client = new Client
            {
                Id = id,
                UserId = "u1",
                Name = "Alvo",
                Email = "alvo@mail.com"
            };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            var repository = new ClientRepository(context);
            var result = await repository.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Alvo", result!.Name);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Client()
        {
            using var context = GetInMemoryContext();
            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                Name = "Antigo",
                Email = "old@mail.com"
            };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            client.Name = "Atualizado";

            var repository = new ClientRepository(context);
            await repository.UpdateAsync(client);

            var updated = await context.Clients.FirstOrDefaultAsync(c => c.Id == client.Id);
            Assert.NotNull(updated);
            Assert.Equal("Atualizado", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Client()
        {
            using var context = GetInMemoryContext();
            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                Name = "Excluir",
                Email = "delete@mail.com"
            };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            var repository = new ClientRepository(context);
            await repository.DeleteAsync(client);

            var exists = await context.Clients.AnyAsync(c => c.Id == client.Id);
            Assert.False(exists);
        }

        [Fact]
        public async Task EmailExistsAsync_Should_Return_True_When_Email_Exists()
        {
            using var context = GetInMemoryContext();
            context.Clients.Add(new Client
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                Name = "Cliente",
                Email = "mail@test.com"
            });
            await context.SaveChangesAsync();

            var repository = new ClientRepository(context);
            var exists = await repository.EmailExistsAsync("u1", "mail@test.com");

            Assert.True(exists);
        }

        [Fact]
        public async Task EmailExistsAsync_Should_Return_False_When_Email_Not_Exists()
        {
            using var context = GetInMemoryContext();
            var repository = new ClientRepository(context);

            var exists = await repository.EmailExistsAsync("u1", "naoexiste@mail.com");

            Assert.False(exists);
        }
    }
}
