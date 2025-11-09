using freelancer_hub_backend.Controllers;
using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace freelancer_hub_backend_tests.Controllers
{
    public class ClientControllerTests
    {
        private readonly Mock<IClientService> _clientServiceMock;
        private readonly Mock<IUserUtils> _userUtilsMock;
        private readonly ClientController _controller;

        public ClientControllerTests()
        {
            _clientServiceMock = new Mock<IClientService>();
            _userUtilsMock = new Mock<IUserUtils>();

            // Mockando o User do HttpContext
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", "test-user-id")
            }));

            _userUtilsMock.Setup(u => u.GetSupabaseUserId(user)).Returns("test-user-id");

            _controller = new ClientController(_clientServiceMock.Object, _userUtilsMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        [Fact]
        public async Task GetClients_ReturnsOk_WithClients()
        {
            var clients = new List<ClientReadDto>
            {
                new ClientReadDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Client1",
                    Email = "client1@test.com",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _clientServiceMock.Setup(s => s.GetClientsAsync("test-user-id"))
                              .ReturnsAsync(clients);

            var result = await _controller.GetClients();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedClients = Assert.IsAssignableFrom<IEnumerable<ClientReadDto>>(okResult.Value);
            Assert.Single(returnedClients);
        }

        [Fact]
        public async Task GetClients_ReturnsUnauthorized_WhenExceptionThrown()
        {
            _clientServiceMock.Setup(s => s.GetClientsAsync("test-user-id"))
                              .ThrowsAsync(new UnauthorizedAccessException("No access"));

            var result = await _controller.GetClients();

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);

            // Serializa e desserializa para dicionário
            var json = System.Text.Json.JsonSerializer.Serialize(unauthorizedResult.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            Assert.NotNull(dict);
            Assert.Equal("No access", dict["message"]);
        }

        [Fact]
        public async Task GetClientById_ReturnsOk_WhenClientExists()
        {
            var client = new ClientReadDto
            {
                Id = Guid.NewGuid(),
                Name = "Client1",
                Email = "client1@test.com",
                CreatedAt = DateTime.UtcNow
            };

            _clientServiceMock.Setup(s => s.GetClientByIdAsync(client.Id))
                              .ReturnsAsync(client);

            var result = await _controller.GetClientById(client.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(client, okResult.Value);
        }

        [Fact]
        public async Task GetClientById_ReturnsNotFound_WhenClientDoesNotExist()
        {
            _clientServiceMock.Setup(s => s.GetClientByIdAsync(It.IsAny<Guid>()))
                              .ReturnsAsync((ClientReadDto)null);

            var result = await _controller.GetClientById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateClient_ReturnsCreatedAtAction_WhenSuccessful()
        {
            var dto = new ClientCreateDto
            {
                Name = "New Client",
                Email = "newclient@test.com",
                Phone = "123456789",
                CompanyName = "Company",
                Notes = "Some notes"
            };

            var createdClient = new ClientReadDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                CompanyName = dto.CompanyName,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _clientServiceMock.Setup(s => s.CreateClientAsync("test-user-id", dto))
                              .ReturnsAsync(createdClient);

            var result = await _controller.CreateClient(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedClient = Assert.IsType<ClientReadDto>(createdResult.Value);
            Assert.Equal(createdClient.Id, returnedClient.Id);
            Assert.Equal(nameof(ClientController.GetClientById), createdResult.ActionName);
        }

        [Fact]
        public async Task CreateClient_ReturnsUnauthorized_WhenUnauthorized()
        {
            _clientServiceMock.Setup(s => s.CreateClientAsync("test-user-id", It.IsAny<ClientCreateDto>()))
                              .ThrowsAsync(new UnauthorizedAccessException());

            var dto = new ClientCreateDto { Name = "New Client", Email = "newclient@test.com" };
            var result = await _controller.CreateClient(dto);

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task UpdateClient_ReturnsNoContent_WhenSuccessful()
        {
            var dto = new ClientUpdateDto
            {
                Name = "Updated",
                Email = "updated@test.com",
                Phone = "987654321",
                CompanyName = "Updated Company",
                Notes = "Updated notes"
            };

            _clientServiceMock.Setup(s => s.UpdateClientAsync(It.IsAny<Guid>(), dto))
                              .Returns(Task.CompletedTask);

            var result = await _controller.UpdateClient(Guid.NewGuid(), dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateClient_ReturnsNotFound_WhenClientDoesNotExist()
        {
            _clientServiceMock.Setup(s => s.UpdateClientAsync(It.IsAny<Guid>(), It.IsAny<ClientUpdateDto>()))
                              .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateClient(Guid.NewGuid(), new ClientUpdateDto
            {
                Name = "Test",
                Email = "test@test.com"
            });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteClient_ReturnsNoContent_WhenSuccessful()
        {
            _clientServiceMock.Setup(s => s.DeleteClientAsync(It.IsAny<Guid>()))
                              .Returns(Task.CompletedTask);

            var result = await _controller.DeleteClient(Guid.NewGuid());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteClient_ReturnsNotFound_WhenClientDoesNotExist()
        {
            _clientServiceMock.Setup(s => s.DeleteClientAsync(It.IsAny<Guid>()))
                              .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeleteClient(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
