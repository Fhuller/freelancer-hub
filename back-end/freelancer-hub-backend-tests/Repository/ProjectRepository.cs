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
    public class ProjectRepositoryTests
    {
        private FreelancerContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FreelancerContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_Project()
        {
            using var context = GetInMemoryContext();
            var repository = new ProjectRepository(context);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                ClientId = Guid.NewGuid(),
                Title = "Website Portfolio",
                Description = "Criação de portfólio pessoal",
                Status = "pendente",
                DueDate = DateTime.UtcNow.AddDays(10),
                HourlyRate = 100,
                TotalHours = 0
            };

            await repository.AddAsync(project);

            var saved = await context.Projects.FirstOrDefaultAsync(p => p.Title == "Website Portfolio");
            Assert.NotNull(saved);
            Assert.Equal("user1", saved!.UserId);
            Assert.Equal(100, saved.HourlyRate);
        }

        [Fact]
        public async Task GetByUserIdAsync_Should_Return_Only_User_Projects_In_Descending_Order()
        {
            using var context = GetInMemoryContext();
            context.Projects.AddRange(
                new Project { Id = Guid.NewGuid(), UserId = "u1", ClientId = Guid.NewGuid(), Title = "Projeto Antigo", CreatedAt = DateTime.UtcNow.AddDays(-3), DueDate = DateTime.UtcNow.AddDays(5) },
                new Project { Id = Guid.NewGuid(), UserId = "u1", ClientId = Guid.NewGuid(), Title = "Projeto Recente", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(10) },
                new Project { Id = Guid.NewGuid(), UserId = "u2", ClientId = Guid.NewGuid(), Title = "Outro Usuário", CreatedAt = DateTime.UtcNow.AddDays(-1), DueDate = DateTime.UtcNow.AddDays(2) }
            );
            await context.SaveChangesAsync();

            var repository = new ProjectRepository(context);
            var result = (await repository.GetByUserIdAsync("u1")).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Projeto Recente", result.First().Title); // ordem decrescente por CreatedAt
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Project()
        {
            using var context = GetInMemoryContext();
            var id = Guid.NewGuid();
            var project = new Project
            {
                Id = id,
                UserId = "u1",
                ClientId = Guid.NewGuid(),
                Title = "E-commerce",
                DueDate = DateTime.UtcNow.AddDays(7)
            };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var repository = new ProjectRepository(context);
            var result = await repository.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("E-commerce", result!.Title);
        }

        [Fact]
        public async Task GetByIdWithClientAsync_Should_Return_Correct_Project()
        {
            using var context = GetInMemoryContext();
            var id = Guid.NewGuid();
            var project = new Project
            {
                Id = id,
                UserId = "u1",
                ClientId = Guid.NewGuid(),
                Title = "API Backend",
                DueDate = DateTime.UtcNow.AddDays(15)
            };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var repository = new ProjectRepository(context);
            var result = await repository.GetByIdWithClientAsync(id);

            Assert.NotNull(result);
            Assert.Equal("API Backend", result!.Title);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_Project()
        {
            using var context = GetInMemoryContext();
            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                ClientId = Guid.NewGuid(),
                Title = "Design UI",
                Status = "pendente",
                DueDate = DateTime.UtcNow.AddDays(3)
            };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            project.Status = "em andamento";
            var repository = new ProjectRepository(context);
            await repository.UpdateAsync(project);

            var updated = await context.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
            Assert.NotNull(updated);
            Assert.Equal("em andamento", updated!.Status);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Project()
        {
            using var context = GetInMemoryContext();
            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                ClientId = Guid.NewGuid(),
                Title = "Landing Page",
                DueDate = DateTime.UtcNow.AddDays(2)
            };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            var repository = new ProjectRepository(context);
            await repository.DeleteAsync(project);

            var exists = await context.Projects.AnyAsync(p => p.Id == project.Id);
            Assert.False(exists);
        }

        [Fact]
        public async Task ClientExistsForUserAsync_Should_Return_True_When_Client_Belongs_To_User()
        {
            using var context = GetInMemoryContext();
            var clientId = Guid.NewGuid();
            context.Clients.Add(new Client
            {
                Id = clientId,
                UserId = "user1",
                Name = "Cliente Teste",
                Email = "cliente@test.com"
            });
            await context.SaveChangesAsync();

            var repository = new ProjectRepository(context);
            var exists = await repository.ClientExistsForUserAsync("user1", clientId);

            Assert.True(exists);
        }

        [Fact]
        public async Task ClientExistsForUserAsync_Should_Return_False_When_Client_Does_Not_Belong_To_User()
        {
            using var context = GetInMemoryContext();
            var clientId = Guid.NewGuid();
            context.Clients.Add(new Client
            {
                Id = clientId,
                UserId = "user2",
                Name = "Outro Cliente",
                Email = "outro@test.com"
            });
            await context.SaveChangesAsync();

            var repository = new ProjectRepository(context);
            var exists = await repository.ClientExistsForUserAsync("user1", clientId);

            Assert.False(exists);
        }

        [Fact]
        public async Task ClientExistsForUserAsync_Should_Return_True_When_ClientId_Is_Null()
        {
            using var context = GetInMemoryContext();
            var repository = new ProjectRepository(context);

            var result = await repository.ClientExistsForUserAsync("user1", null);

            Assert.True(result);
        }
    }
}
