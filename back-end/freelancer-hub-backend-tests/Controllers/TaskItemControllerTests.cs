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
    public class TaskItemControllerTests
    {
        private readonly Mock<ITaskItemService> _taskItemServiceMock;
        private readonly Mock<IUserUtils> _userUtilsMock;
        private readonly TaskItemController _controller;

        public TaskItemControllerTests()
        {
            _taskItemServiceMock = new Mock<ITaskItemService>();
            _userUtilsMock = new Mock<IUserUtils>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", "test-user-id")
            }));

            _userUtilsMock.Setup(u => u.GetJWTUserID(user)).Returns("test-user-id");

            _controller = new TaskItemController(_taskItemServiceMock.Object, _userUtilsMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        [Fact]
        public async Task GetTaskItemsByProject_ReturnsOk()
        {
            var projectId = Guid.NewGuid();
            var tasks = new List<TaskItemDto>
            {
                new TaskItemDto { Id = Guid.NewGuid(), Title = "Task 1" }
            };

            _taskItemServiceMock.Setup(s => s.GetTaskItemsByProjectAsync("test-user-id", projectId))
                                .ReturnsAsync(tasks);

            var result = await _controller.GetTaskItemsByProject(projectId);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<TaskItemDto>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetTaskItemsByProject_ReturnsUnauthorized_WhenUnauthorized()
        {
            var projectId = Guid.NewGuid();

            _taskItemServiceMock.Setup(s => s.GetTaskItemsByProjectAsync("test-user-id", projectId))
                                .ThrowsAsync(new UnauthorizedAccessException("no access"));

            var result = await _controller.GetTaskItemsByProject(projectId);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var json = System.Text.Json.JsonSerializer.Serialize(unauthorized.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("no access", dict["message"]);
        }

        [Fact]
        public async Task GetTaskItemsByProject_ReturnsBadRequest_WhenInvalidArgument()
        {
            var projectId = Guid.NewGuid();

            _taskItemServiceMock.Setup(s => s.GetTaskItemsByProjectAsync("test-user-id", projectId))
                                .ThrowsAsync(new ArgumentException("invalid"));

            var result = await _controller.GetTaskItemsByProject(projectId);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            var json = System.Text.Json.JsonSerializer.Serialize(bad.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("invalid", dict["message"]);
        }

        [Fact]
        public async Task GetTaskItemById_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            var dto = new TaskItemDto { Id = id, Title = "Test" };

            _taskItemServiceMock.Setup(s => s.GetTaskItemByIdAsync(id))
                                .ReturnsAsync(dto);

            var result = await _controller.GetTaskItemById(id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetTaskItemById_ReturnsNotFound()
        {
            _taskItemServiceMock.Setup(s => s.GetTaskItemByIdAsync(It.IsAny<Guid>()))
                                .ReturnsAsync((TaskItemDto)null);

            var result = await _controller.GetTaskItemById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateTaskItem_ReturnsCreated()
        {
            var dto = new TaskItemCreateDto { Title = "Test" };
            var created = new TaskItemDto { Id = Guid.NewGuid(), Title = "Test" };

            _taskItemServiceMock.Setup(s => s.CreateTaskItemAsync("test-user-id", dto))
                                .ReturnsAsync(created);

            var result = await _controller.CreateTaskItem(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created.Id, ((TaskItemDto)createdResult.Value).Id);
        }

        [Fact]
        public async Task CreateTaskItem_ReturnsUnauthorized()
        {
            var dto = new TaskItemCreateDto { Title = "Test" };

            _taskItemServiceMock.Setup(s => s.CreateTaskItemAsync("test-user-id", dto))
                                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.CreateTaskItem(dto);

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task CreateTaskItem_ReturnsBadRequest()
        {
            var dto = new TaskItemCreateDto { Title = "Bad" };

            _taskItemServiceMock.Setup(s => s.CreateTaskItemAsync("test-user-id", dto))
                                .ThrowsAsync(new ArgumentException("wrong"));

            var result = await _controller.CreateTaskItem(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            var json = System.Text.Json.JsonSerializer.Serialize(bad.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("wrong", dict["message"]);
        }

        [Fact]
        public async Task UpdateTaskItem_ReturnsNoContent()
        {
            _taskItemServiceMock.Setup(s => s.UpdateTaskItemAsync(It.IsAny<Guid>(), It.IsAny<TaskItemUpdateDto>()))
                                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateTaskItem(Guid.NewGuid(), new TaskItemUpdateDto());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateTaskItem_ReturnsNotFound()
        {
            _taskItemServiceMock.Setup(s => s.UpdateTaskItemAsync(It.IsAny<Guid>(), It.IsAny<TaskItemUpdateDto>()))
                                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateTaskItem(Guid.NewGuid(), new TaskItemUpdateDto());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTaskItem_ReturnsBadRequest()
        {
            _taskItemServiceMock.Setup(s => s.UpdateTaskItemAsync(It.IsAny<Guid>(), It.IsAny<TaskItemUpdateDto>()))
                                .ThrowsAsync(new ArgumentException("error"));

            var result = await _controller.UpdateTaskItem(Guid.NewGuid(), new TaskItemUpdateDto());

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var json = System.Text.Json.JsonSerializer.Serialize(bad.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("error", dict["message"]);
        }

        [Fact]
        public async Task DeleteTaskItem_ReturnsNoContent()
        {
            _taskItemServiceMock.Setup(s => s.DeleteTaskItemAsync(It.IsAny<Guid>()))
                                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteTaskItem(Guid.NewGuid());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTaskItem_ReturnsNotFound()
        {
            _taskItemServiceMock.Setup(s => s.DeleteTaskItemAsync(It.IsAny<Guid>()))
                                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeleteTaskItem(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
