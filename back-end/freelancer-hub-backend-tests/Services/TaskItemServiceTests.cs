using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Repository;
using freelancer_hub_backend.Services;
using Moq;
using Xunit;

namespace freelancer_hub_backend_tests.Services
{
    public class TaskItemServiceTests
    {
        private readonly Mock<ITaskItemRepository> _mockRepo;
        private readonly TaskItemService _service;

        public TaskItemServiceTests()
        {
            _mockRepo = new Mock<ITaskItemRepository>();
            _service = new TaskItemService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetTaskItemsByProjectAsync_ShouldReturnTaskItems()
        {
            var userId = "user123";
            var projectId = Guid.NewGuid();

            _mockRepo.Setup(r => r.ProjectExistsForUserAsync(userId, projectId))
                     .ReturnsAsync(true);

            _mockRepo.Setup(r => r.GetByProjectIdAsync(projectId))
                     .ReturnsAsync(new List<TaskItem>
                     {
                         new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 1" },
                         new TaskItem { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Task 2" }
                     });

            var result = await _service.GetTaskItemsByProjectAsync(userId, projectId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(r => r.GetByProjectIdAsync(projectId), Times.Once);
        }

        [Fact]
        public async Task GetTaskItemsByProjectAsync_ShouldThrow_WhenUserIdIsNull()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.GetTaskItemsByProjectAsync(null, Guid.NewGuid()));
        }

        [Fact]
        public async Task GetTaskItemsByProjectAsync_ShouldThrow_WhenProjectNotBelongsToUser()
        {
            var userId = "user123";
            var projectId = Guid.NewGuid();

            _mockRepo.Setup(r => r.ProjectExistsForUserAsync(userId, projectId))
                     .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetTaskItemsByProjectAsync(userId, projectId));
        }

        [Fact]
        public async Task GetTaskItemByIdAsync_ShouldReturnTaskItem()
        {
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByIdWithProjectAsync(id))
                     .ReturnsAsync(new TaskItem { Id = id, Title = "Tarefa X" });

            var result = await _service.GetTaskItemByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Tarefa X", result.Title);
        }

        [Fact]
        public async Task GetTaskItemByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdWithProjectAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((TaskItem)null);

            var result = await _service.GetTaskItemByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTaskItemAsync_ShouldCreate_WhenValid()
        {
            var userId = "user123";
            var projectId = Guid.NewGuid();
            var dto = new TaskItemCreateDto
            {
                ProjectId = projectId,
                Title = "Nova Tarefa",
                Status = "Pendente",
                DueDate = DateTime.UtcNow
            };

            _mockRepo.Setup(r => r.ProjectExistsForUserAsync(userId, projectId))
                     .ReturnsAsync(true);

            var result = await _service.CreateTaskItemAsync(userId, dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Title, result.Title);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task CreateTaskItemAsync_ShouldThrow_WhenUserNotAuthenticated()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.CreateTaskItemAsync(null, new TaskItemCreateDto()));
        }

        [Fact]
        public async Task CreateTaskItemAsync_ShouldThrow_WhenProjectInvalid()
        {
            var userId = "user123";
            var dto = new TaskItemCreateDto
            {
                ProjectId = Guid.NewGuid(),
                Title = "Teste",
                Status = "Pendente"
            };

            _mockRepo.Setup(r => r.ProjectExistsForUserAsync(userId, dto.ProjectId))
                     .ReturnsAsync(false);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateTaskItemAsync(userId, dto));
        }

        [Fact]
        public async Task UpdateTaskItemAsync_ShouldUpdate_WhenValid()
        {
            var id = Guid.NewGuid();
            var dto = new TaskItemUpdateDto
            {
                Title = "Atualizado",
                Description = "Nova descrição",
                Status = "Em Andamento"
            };

            _mockRepo.Setup(r => r.GetByIdWithProjectAsync(id))
                     .ReturnsAsync(new TaskItem { Id = id, Title = "Antigo" });

            await _service.UpdateTaskItemAsync(id, dto);

            _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTaskItemAsync_ShouldThrow_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdWithProjectAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((TaskItem)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateTaskItemAsync(Guid.NewGuid(), new TaskItemUpdateDto()));
        }

        [Fact]
        public async Task DeleteTaskItemAsync_ShouldDelete_WhenExists()
        {
            var id = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync(new TaskItem { Id = id });

            await _service.DeleteTaskItemAsync(id);

            _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<TaskItem>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTaskItemAsync_ShouldThrow_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                     .ReturnsAsync((TaskItem)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.DeleteTaskItemAsync(Guid.NewGuid()));
        }
    }
}
