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
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            var users = new List<User>
            {
                new() { Id = "1", Name = "Felipe", Email = "felipe@example.com", CreatedAt = DateTime.UtcNow },
                new() { Id = "2", Name = "João", Email = "joao@example.com", CreatedAt = DateTime.UtcNow }
            };

            _userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var result = await _userService.GetAllUsersAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Email == "felipe@example.com");
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            var user = new User { Id = "1", Name = "Felipe", Email = "felipe@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync("1");

            Assert.NotNull(result);
            Assert.Equal("Felipe", result.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("x")).ReturnsAsync((User)null);

            var result = await _userService.GetUserByIdAsync("x");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldThrow_WhenIdIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserByIdAsync(""));
        }

        [Fact]
        public async Task CreateOrGetUserAsync_ShouldReturnExistingUser_WhenExists()
        {
            var user = new User { Id = "abc", Name = "Felipe", Email = "felipe@example.com" };
            var dto = new UserCreateDto { Name = "Outro", Email = "outro@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync(user);

            var result = await _userService.CreateOrGetUserAsync("abc", dto);

            Assert.Equal("Felipe", result.Name);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrGetUserAsync_ShouldCreateNewUser_WhenNotExists()
        {
            var dto = new UserCreateDto { Name = "Felipe", Email = "felipe@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync("abc")).ReturnsAsync((User)null);
            _userRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email, "abc")).ReturnsAsync(false);

            var result = await _userService.CreateOrGetUserAsync("abc", dto);

            Assert.Equal(dto.Name, result.Name);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrGetUserAsync_ShouldThrow_WhenUserIdIsNull()
        {
            var dto = new UserCreateDto { Name = "Felipe", Email = "felipe@example.com" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.CreateOrGetUserAsync("", dto));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateSuccessfully_WhenValid()
        {
            var user = new User { Id = "1", Name = "Felipe", Email = "felipe@example.com" };
            var dto = new UserUpdateDto { Name = "Novo Nome", Email = "novo@email.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email, "1")).ReturnsAsync(false);

            await _userService.UpdateUserAsync("1", dto);

            _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Name == "Novo Nome")), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("x")).ReturnsAsync((User)null);

            var dto = new UserUpdateDto { Name = "Felipe", Email = "felipe@example.com" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateUserAsync("x", dto));
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDelete_WhenExists()
        {
            var user = new User { Id = "1", Name = "Felipe", Email = "felipe@example.com" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            await _userService.DeleteUserAsync("1");

            _userRepositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrow_WhenNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("x")).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeleteUserAsync("x"));
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldReturnUser_WhenAuthenticated()
        {
            var user = new User { Id = "1", Name = "Felipe", Email = "felipe@example.com" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            var result = await _userService.GetCurrentUserAsync("1");

            Assert.Equal("Felipe", result.Name);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.GetCurrentUserAsync(""));
        }

        [Fact]
        public async Task UpdateLanguageAsync_ShouldUpdateLanguage_WhenValid()
        {
            var user = new User { Id = "1", Name = "Felipe", Email = "felipe@example.com" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync(user);

            await _userService.UpdateLanguageAsync("1", "en");

            Assert.Equal("en", user.Language);
            _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateLanguageAsync_ShouldThrow_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("1")).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateLanguageAsync("1", "en"));
        }
    }
}
