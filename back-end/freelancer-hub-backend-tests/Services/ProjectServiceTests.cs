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
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _repositoryMock;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _repositoryMock = new Mock<IProjectRepository>();
            _service = new ProjectService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetProjectsAsync_ShouldReturnProjects_WhenUserIdIsValid()
        {
            var userId = "user1";
            var projects = new List<Project>
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    ClientId = Guid.NewGuid(),
                    Title = "Sistema de gestão",
                    Status = "Em Andamento",
                    DueDate = DateTime.UtcNow.AddDays(10)
                }
            };

            _repositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(projects);

            var result = await _service.GetProjectsAsync(userId);

            Assert.Single(result);
            Assert.Equal("Sistema de gestão", result.First().Title);
        }

        [Fact]
        public async Task GetProjectsAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.GetProjectsAsync(string.Empty));
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnProject_WhenFound()
        {
            var id = Guid.NewGuid();
            var project = new Project
            {
                Id = id,
                UserId = "user1",
                ClientId = Guid.NewGuid(),
                Title = "App Mobile",
                Status = "Concluído",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            _repositoryMock.Setup(r => r.GetByIdWithClientAsync(id)).ReturnsAsync(project);

            var result = await _service.GetProjectByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("App Mobile", result.Title);
            Assert.Equal("Concluído", result.Status);
        }

        [Fact]
        public async Task GetProjectByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdWithClientAsync(It.IsAny<Guid>())).ReturnsAsync((Project)null);

            var result = await _service.GetProjectByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldCreate_WhenValid()
        {
            var userId = "user1";
            var clientId = Guid.NewGuid();

            var dto = new ProjectCreateDto
            {
                UserId = userId,
                ClientId = clientId,
                Title = "Novo Projeto",
                Description = "Descrição teste",
                Status = "Pendente",
                DueDate = DateTime.UtcNow.AddDays(3),
                HourlyRate = 100
            };

            _repositoryMock.Setup(r => r.ClientExistsForUserAsync(userId, clientId)).ReturnsAsync(true);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

            var result = await _service.CreateProjectAsync(userId, dto);

            Assert.NotNull(result);
            Assert.Equal("Novo Projeto", result.Title);
            Assert.Equal("Pendente", result.Status);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            var dto = new ProjectCreateDto
            {
                UserId = "",
                ClientId = Guid.NewGuid(),
                Title = "Teste",
                Status = "Pendente",
                DueDate = DateTime.UtcNow.AddDays(2)
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateProjectAsync("", dto));
        }

        [Theory]
        [InlineData("", "Pendente", "O título do projeto é obrigatório.")]
        [InlineData("Projeto", "", "O status do projeto é obrigatório.")]
        [InlineData("Projeto", "StatusInvalido", "Status inválido")]
        public async Task CreateProjectAsync_ShouldThrow_OnInvalidFields(string title, string status, string expected)
        {
            var dto = new ProjectCreateDto
            {
                UserId = "user1",
                ClientId = Guid.NewGuid(),
                Title = title,
                Status = status,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            _repositoryMock.Setup(r => r.ClientExistsForUserAsync(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProjectAsync("user1", dto));
            Assert.Contains(expected, ex.Message);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldThrow_WhenClientNotExists()
        {
            var dto = new ProjectCreateDto
            {
                UserId = "user1",
                ClientId = Guid.NewGuid(),
                Title = "Projeto",
                Status = "Pendente",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            _repositoryMock.Setup(r => r.ClientExistsForUserAsync("user1", dto.ClientId)).ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProjectAsync("user1", dto));

            Assert.Contains("Cliente não encontrado", ex.Message);
        }

        [Fact]
        public async Task CreateProjectAsync_ShouldThrow_WhenDueDateIsPast()
        {
            var dto = new ProjectCreateDto
            {
                UserId = "user1",
                ClientId = Guid.NewGuid(),
                Title = "Projeto",
                Status = "Pendente",
                DueDate = DateTime.UtcNow.AddDays(-1)
            };

            _repositoryMock.Setup(r => r.ClientExistsForUserAsync("user1", dto.ClientId)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProjectAsync("user1", dto));

            Assert.Contains("A data de vencimento não pode ser anterior", ex.Message);
        }

        [Fact]
        public async Task UpdateProjectAsync_ShouldUpdate_WhenValid()
        {
            var id = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var project = new Project
            {
                Id = id,
                UserId = "user1",
                ClientId = clientId,
                Title = "Antigo",
                Status = "Pendente",
                DueDate = DateTime.UtcNow.AddDays(5)
            };

            var dto = new ProjectUpdateDto
            {
                Title = "Atualizado",
                Description = "Nova descrição",
                Status = "Em Andamento",
                ClientId = clientId,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(project);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Project>())).Returns(Task.CompletedTask);

            await _service.UpdateProjectAsync(id, dto);

            Assert.Equal("Atualizado", project.Title);
            Assert.Equal("Em Andamento", project.Status);
            _repositoryMock.Verify(r => r.UpdateAsync(project), Times.Once);
        }

        [Fact]
        public async Task UpdateProjectAsync_ShouldThrow_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Project)null);

            var dto = new ProjectUpdateDto
            {
                Title = "Teste",
                Status = "Pendente"
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateProjectAsync(Guid.NewGuid(), dto));
        }

        [Theory]
        [InlineData("", "Pendente", "O título do projeto é obrigatório.")]
        [InlineData("Projeto", "", "O status do projeto é obrigatório.")]
        [InlineData("Projeto", "Invalido", "Status inválido")]
        public async Task UpdateProjectAsync_ShouldThrow_OnInvalidFields(string title, string status, string expected)
        {
            var id = Guid.NewGuid();
            var project = new Project { Id = id, UserId = "user1", ClientId = Guid.NewGuid() };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(project);

            var dto = new ProjectUpdateDto
            {
                Title = title,
                Status = status
            };

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProjectAsync(id, dto));
            Assert.Contains(expected, ex.Message);
        }

        [Fact]
        public async Task UpdateProjectAsync_ShouldThrow_WhenClientNotExists()
        {
            var id = Guid.NewGuid();
            var project = new Project { Id = id, UserId = "user1", ClientId = Guid.NewGuid() };

            var dto = new ProjectUpdateDto
            {
                ClientId = Guid.NewGuid(),
                Title = "Novo título",
                Status = "Em Andamento"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(project);
            _repositoryMock.Setup(r => r.ClientExistsForUserAsync("user1", dto.ClientId)).ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateProjectAsync(id, dto));

            Assert.Contains("Cliente não encontrado", ex.Message);
        }

        [Fact]
        public async Task DeleteProjectAsync_ShouldDelete_WhenExists()
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                Title = "Projeto a remover"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(project.Id)).ReturnsAsync(project);

            await _service.DeleteProjectAsync(project.Id);

            _repositoryMock.Verify(r => r.DeleteAsync(project), Times.Once);
        }

        [Fact]
        public async Task DeleteProjectAsync_ShouldThrow_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Project)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteProjectAsync(Guid.NewGuid()));
        }
    }
}
