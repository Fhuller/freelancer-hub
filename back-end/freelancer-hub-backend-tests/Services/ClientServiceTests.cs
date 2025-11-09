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
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _repositoryMock;
        private readonly ClientService _service;

        public ClientServiceTests()
        {
            _repositoryMock = new Mock<IClientRepository>();
            _service = new ClientService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetClientsAsync_ShouldReturnClients_WhenUserIdIsValid()
        {
            var userId = "user123";
            var clients = new List<Client>
            {
                new Client
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "John",
                    Email = "john@test.com",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _repositoryMock.Setup(r => r.GetByUserIdAsync(userId))
                           .ReturnsAsync(clients);

            var result = await _service.GetClientsAsync(userId);

            Assert.Single(result);
            Assert.Equal("John", result.First().Name);
            Assert.Equal("john@test.com", result.First().Email);
        }

        [Fact]
        public async Task GetClientsAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.GetClientsAsync(string.Empty));
        }

        [Fact]
        public async Task GetClientByIdAsync_ShouldReturnClient_WhenFound()
        {
            var id = Guid.NewGuid();
            var client = new Client
            {
                Id = id,
                UserId = "user1",
                Name = "Jane Doe",
                Email = "jane@test.com"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(client);

            var result = await _service.GetClientByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal("Jane Doe", result.Name);
            Assert.Equal("jane@test.com", result.Email);
        }

        [Fact]
        public async Task GetClientByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client)null);

            var result = await _service.GetClientByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateClientAsync_ShouldCreateClient_WhenValid()
        {
            var userId = "user1";
            var dto = new ClientCreateDto
            {
                Name = "Carlos",
                Email = "carlos@test.com",
                Phone = "9999",
                CompanyName = "Empresa X",
                Notes = "Teste"
            };

            _repositoryMock.Setup(r => r.EmailExistsAsync(userId, dto.Email)).ReturnsAsync(false);

            Client addedClient = null;
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Client>()))
                .Callback<Client>(c => addedClient = c)
                .Returns(Task.CompletedTask);

            var result = await _service.CreateClientAsync(userId, dto);

            Assert.NotNull(result);
            Assert.Equal("Carlos", result.Name);
            Assert.Equal("carlos@test.com", result.Email);
            Assert.Equal(addedClient.Id, result.Id);
            Assert.Equal(userId, addedClient.UserId);
            Assert.NotEqual(default, addedClient.CreatedAt);
        }

        [Fact]
        public async Task CreateClientAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            var dto = new ClientCreateDto { Name = "Carlos", Email = "carlos@test.com" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.CreateClientAsync(string.Empty, dto));
        }

        [Theory]
        [InlineData("", "email@test.com", "O nome do cliente é obrigatório.")]
        [InlineData("Name", "", "O email do cliente é obrigatório.")]
        [InlineData("Name", "invalidEmail", "O email fornecido é inválido.")]
        public async Task CreateClientAsync_ShouldThrow_OnInvalidFields(string name, string email, string expectedMessage)
        {
            var userId = "user1";
            var dto = new ClientCreateDto { Name = name, Email = email };

            _repositoryMock.Setup(r => r.EmailExistsAsync(userId, It.IsAny<string>())).ReturnsAsync(false);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateClientAsync(userId, dto));

            Assert.Contains(expectedMessage, ex.Message);
        }

        [Fact]
        public async Task CreateClientAsync_ShouldThrow_WhenEmailAlreadyExists()
        {
            var userId = "user1";
            var dto = new ClientCreateDto { Name = "John", Email = "john@test.com" };

            _repositoryMock.Setup(r => r.EmailExistsAsync(userId, dto.Email)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateClientAsync(userId, dto));

            Assert.Contains("Já existe um cliente com este email.", ex.Message);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldUpdate_WhenClientExists()
        {
            var id = Guid.NewGuid();
            var client = new Client
            {
                Id = id,
                UserId = "user1",
                Name = "Old Name",
                Email = "old@test.com"
            };

            var dto = new ClientUpdateDto
            {
                Name = "New Name",
                Email = "new@test.com",
                Phone = "8888",
                CompanyName = "Company",
                Notes = "Updated"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(client);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Client>())).Returns(Task.CompletedTask);

            await _service.UpdateClientAsync(id, dto);

            Assert.Equal("New Name", client.Name);
            Assert.Equal("new@test.com", client.Email);
            Assert.Equal("8888", client.Phone);
            Assert.Equal("Company", client.CompanyName);
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Client>(c => c.Id == id)), Times.Once);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldThrow_WhenClientNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client)null);

            var dto = new ClientUpdateDto();

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateClientAsync(Guid.NewGuid(), dto));
        }

        [Fact]
        public async Task DeleteClientAsync_ShouldDelete_WhenClientExists()
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = "user1",
                Name = "Client X",
                Email = "x@test.com"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(client.Id)).ReturnsAsync(client);

            await _service.DeleteClientAsync(client.Id);

            _repositoryMock.Verify(r => r.DeleteAsync(client), Times.Once);
        }

        [Fact]
        public async Task DeleteClientAsync_ShouldThrow_WhenClientNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Client)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteClientAsync(Guid.NewGuid()));
        }
    }
}
