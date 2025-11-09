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
    public class TaskItemRepositoryTests
    {
        private FreelancerContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FreelancerContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Add_TaskItem()
        {
            using var context = GetInMemoryContext();
            var repository = new TaskItemRepository(context);

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                Title = "Criar layout Figma",
                Description = "Montar tela inicial",
                Status = "pendente",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            await repository.AddAsync(task);

            var saved = await context.TaskItems.FirstOrDefaultAsync(t => t.Title == "Criar layout Figma");
            Assert.NotNull(saved);
            Assert.Equal("pendente", saved!.Status);
            Assert.Equal("Montar tela inicial", saved.Description);
        }

        [Fact]
        public async Task GetByProjectIdAsync_Should_Return_Ordered_Tasks()
        {
            using var context = GetInMemoryContext();
            var projectId = Guid.NewGuid();

            context.TaskItems.AddRange(
                new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task A", DueDate = DateTime.UtcNow.AddDays(3), CreatedAt = DateTime.UtcNow },
                new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task B", DueDate = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
                new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task C", DueDate = DateTime.UtcNow.AddDays(1), CreatedAt = DateTime.UtcNow.AddMinutes(5) },
                new TaskItem { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Title = "Outra Task", DueDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var repository = new TaskItemRepository(context);
            var result = (await repository.GetByProjectIdAsync(projectId)).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal("Task B", result.First().Title); // ordena por DueDate asc, depois CreatedAt desc
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_TaskItem()
        {
            using var context = GetInMemoryContext();
            var id = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = id,
                ProjectId = Guid.NewGuid(),
                Title = "Implementar autenticação",
                DueDate = DateTime.UtcNow.AddDays(5)
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var repository = new TaskItemRepository(context);
            var result = await repository.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Implementar autenticação", result!.Title);
        }

        [Fact]
        public async Task GetByIdWithProjectAsync_Should_Return_Correct_TaskItem()
        {
            using var context = GetInMemoryContext();
            var id = Guid.NewGuid();
            var task = new TaskItem
            {
                Id = id,
                ProjectId = Guid.NewGuid(),
                Title = "Criar API de tarefas",
                DueDate = DateTime.UtcNow.AddDays(2)
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var repository = new TaskItemRepository(context);
            var result = await repository.GetByIdWithProjectAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Criar API de tarefas", result!.Title);
        }

        [Fact]
        public async Task UpdateAsync_Should_Modify_TaskItem()
        {
            using var context = GetInMemoryContext();
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                Title = "Revisar documentação",
                Status = "pendente",
                DueDate = DateTime.UtcNow.AddDays(1)
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            task.Status = "concluída";
            var repository = new TaskItemRepository(context);
            await repository.UpdateAsync(task);

            var updated = await context.TaskItems.FirstOrDefaultAsync(t => t.Id == task.Id);
            Assert.NotNull(updated);
            Assert.Equal("concluída", updated!.Status);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_TaskItem()
        {
            using var context = GetInMemoryContext();
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                Title = "Excluir tarefa",
                DueDate = DateTime.UtcNow
            };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var repository = new TaskItemRepository(context);
            await repository.DeleteAsync(task);

            var exists = await context.TaskItems.AnyAsync(t => t.Id == task.Id);
            Assert.False(exists);
        }

        [Fact]
        public async Task ProjectExistsForUserAsync_Should_Return_True_When_Project_Belongs_To_User()
        {
            using var context = GetInMemoryContext();
            var projectId = Guid.NewGuid();
            context.Projects.Add(new Project
            {
                Id = projectId,
                UserId = "user1",
                ClientId = Guid.NewGuid(),
                Title = "Sistema de Tarefas",
                DueDate = DateTime.UtcNow.AddDays(10)
            });
            await context.SaveChangesAsync();

            var repository = new TaskItemRepository(context);
            var exists = await repository.ProjectExistsForUserAsync("user1", projectId);

            Assert.True(exists);
        }

        [Fact]
        public async Task ProjectExistsForUserAsync_Should_Return_False_When_Project_Not_Belongs_To_User()
        {
            using var context = GetInMemoryContext();
            var projectId = Guid.NewGuid();
            context.Projects.Add(new Project
            {
                Id = projectId,
                UserId = "user2",
                ClientId = Guid.NewGuid(),
                Title = "Projeto externo",
                DueDate = DateTime.UtcNow.AddDays(5)
            });
            await context.SaveChangesAsync();

            var repository = new TaskItemRepository(context);
            var exists = await repository.ProjectExistsForUserAsync("user1", projectId);

            Assert.False(exists);
        }
    }
}
