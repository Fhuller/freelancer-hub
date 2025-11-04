using freelancer_hub_backend.Controllers;
using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace freelancer_hub_backend_tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IUserUtils> _userUtilsMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _userUtilsMock = new Mock<IUserUtils>();

            var claimsUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "test-user-id")
            }));

            _userUtilsMock.Setup(u => u.GetSupabaseUserId(claimsUser))
                          .Returns("test-user-id");

            _controller = new UserController(_userServiceMock.Object, _userUtilsMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsUser }
                }
            };
        }

        [Fact]
        public async Task GetUsers_ReturnsOk()
        {
            var users = new List<UserDto>
            {
                new UserDto { Id = "1", Email = "a@test.com" }
            };

            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _controller.GetUsers();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<UserDto>>(ok.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsOk()
        {
            var user = new UserDto { Id = "test-user-id" };
            _userServiceMock.Setup(s => s.GetUserByIdAsync("test-user-id")).ReturnsAsync(user);

            var result = await _controller.GetUserById("test-user-id");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, ok.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnsNotFound()
        {
            _userServiceMock.Setup(s => s.GetUserByIdAsync("x")).ReturnsAsync((UserDto)null);

            var result = await _controller.GetUserById("x");

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetUserById_ReturnsBadRequest()
        {
            _userServiceMock.Setup(s => s.GetUserByIdAsync("x"))
                .ThrowsAsync(new ArgumentException("Invalid id"));

            var result = await _controller.GetUserById("x");

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsOk()
        {
            var user = new UserDto { Id = "test-user-id" };
            _userServiceMock.Setup(s => s.GetCurrentUserAsync("test-user-id"))
                            .ReturnsAsync(user);

            var result = await _controller.GetCurrentUser();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(user, ok.Value);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsUnauthorized()
        {
            _userServiceMock.Setup(s => s.GetCurrentUserAsync("test-user-id"))
                .ThrowsAsync(new UnauthorizedAccessException("nope"));

            var result = await _controller.GetCurrentUser();

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateOrGetUser_ReturnsCreated()
        {
            var dto = new UserCreateDto { Email = "a@test.com" };
            var user = new UserDto { Id = "test-user-id", CreatedAt = DateTime.UtcNow };

            _userServiceMock.Setup(s => s.CreateOrGetUserAsync("test-user-id", dto))
                             .ReturnsAsync(user);

            _userServiceMock.Setup(s => s.GetUserByIdAsync("test-user-id"))
                             .ReturnsAsync(user);

            var result = await _controller.CreateOrGetUser(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(user, created.Value);
        }

        [Fact]
        public async Task CreateOrGetUser_ReturnsOk_WhenAlreadyExists()
        {
            var dto = new UserCreateDto { Email = "x@test.com" };
            var existing = new UserDto { Id = "test-user-id", CreatedAt = DateTime.UtcNow.AddMinutes(-5) };
            var returned = new UserDto { Id = "test-user-id", CreatedAt = DateTime.UtcNow };

            _userServiceMock.Setup(s => s.CreateOrGetUserAsync("test-user-id", dto))
                            .ReturnsAsync(returned);

            _userServiceMock.Setup(s => s.GetUserByIdAsync("test-user-id"))
                            .ReturnsAsync(existing);

            var result = await _controller.CreateOrGetUser(dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(returned, ok.Value);
        }

        [Fact]
        public async Task CreateOrGetUser_ReturnsUnauthorized()
        {
            _userServiceMock.Setup(s => s.CreateOrGetUserAsync("test-user-id", It.IsAny<UserCreateDto>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.CreateOrGetUser(new UserCreateDto());

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateOrGetUser_ReturnsBadRequest()
        {
            _userServiceMock.Setup(s => s.CreateOrGetUserAsync("test-user-id", It.IsAny<UserCreateDto>()))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.CreateOrGetUser(new UserCreateDto());

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent()
        {
            var dto = new UserUpdateDto();
            _userServiceMock.Setup(s => s.UpdateUserAsync("test-user-id", dto))
                            .Returns(Task.CompletedTask);

            var result = await _controller.UpdateUser("test-user-id", dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsForbid()
        {
            var result = await _controller.UpdateUser("another-user", new UserUpdateDto());

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound()
        {
            _userServiceMock.Setup(s => s.UpdateUserAsync("test-user-id", It.IsAny<UserUpdateDto>()))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateUser("test-user-id", new UserUpdateDto());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest()
        {
            _userServiceMock.Setup(s => s.UpdateUserAsync("test-user-id", It.IsAny<UserUpdateDto>()))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.UpdateUser("test-user-id", new UserUpdateDto());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsUnauthorized()
        {
            _userServiceMock.Setup(s => s.UpdateUserAsync("test-user-id", It.IsAny<UserUpdateDto>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.UpdateUser("test-user-id", new UserUpdateDto());

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent()
        {
            _userServiceMock.Setup(s => s.DeleteUserAsync("test-user-id"))
                            .Returns(Task.CompletedTask);

            var result = await _controller.DeleteUser("test-user-id");

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsForbid()
        {
            var result = await _controller.DeleteUser("another-user");

            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound()
        {
            _userServiceMock.Setup(s => s.DeleteUserAsync("test-user-id"))
                            .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeleteUser("test-user-id");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsBadRequest()
        {
            _userServiceMock.Setup(s => s.DeleteUserAsync("test-user-id"))
                .ThrowsAsync(new ArgumentException());

            var result = await _controller.DeleteUser("test-user-id");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateLanguage_ReturnsOk()
        {
            _userServiceMock.Setup(s => s.UpdateLanguageAsync("test-user-id", "pt"))
                            .Returns(Task.CompletedTask);

            var result = await _controller.UpdateLanguage("test-user-id", "pt");

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value;

            var language = value.GetType().GetProperty("language").GetValue(value);
            var id = value.GetType().GetProperty("id").GetValue(value);

            Assert.Equal("pt", language);
            Assert.Equal("test-user-id", id);
        }

        [Fact]
        public async Task UpdateLanguage_ReturnsNotFound()
        {
            _userServiceMock.Setup(s => s.UpdateLanguageAsync("test-user-id", "pt"))
                            .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateLanguage("test-user-id", "pt");

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
