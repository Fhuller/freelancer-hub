using freelancer_hub_backend;
using freelancer_hub_backend.Controllers;
using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace freelancer_hub_backend_tests.Controllers
{
    public class ProjectControllerTests
    {
        private readonly Mock<IProjectService> _projectServiceMock;
        private readonly Mock<IBlobStorageService> _blobServiceMock;
        private readonly Mock<IUserUtils> _userUtilsMock;
        private readonly FreelancerContext _context;

        public ProjectControllerTests()
        {
            _projectServiceMock = new Mock<IProjectService>();
            _blobServiceMock = new Mock<IBlobStorageService>();
            _userUtilsMock = new Mock<IUserUtils>();

            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new FreelancerContext(options);
        }

        private ProjectController CreateController()
        {
            return new ProjectController(
                _projectServiceMock.Object,
                _blobServiceMock.Object,
                _context,
                _userUtilsMock.Object
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetProjects_ReturnsOk()
        {
            var controller = CreateController();
            _userUtilsMock.Setup(x => x.GetJWTUserID(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns("user123");
            _projectServiceMock.Setup(s => s.GetProjectsAsync("user123"))
                .ReturnsAsync(new List<ProjectDto>());

            var result = await controller.GetProjects();

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateProject_ReturnsCreated()
        {
            var controller = CreateController();

            var dto = new ProjectCreateDto
            {
                Title = "Test",
                Description = "Desc",
                Status = "pendente",
                ClientId = Guid.NewGuid(),
                UserId = "user123"
            };

            var created = new ProjectDto { Id = Guid.NewGuid(), Title = "Test", UserId = "user123" };

            _userUtilsMock.Setup(x => x.GetJWTUserID(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Returns("user123");
            _projectServiceMock.Setup(s => s.CreateProjectAsync("user123", dto))
                .ReturnsAsync(created);

            var result = await controller.CreateProject(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(ProjectController.GetProjectById), createdResult.ActionName);
        }

        [Fact]
        public async Task GetProjectById_ReturnsNotFound()
        {
            var controller = CreateController();
            _projectServiceMock.Setup(s => s.GetProjectByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ProjectDto)null);

            var result = await controller.GetProjectById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateProjectHours_UpdatesCorrectly()
        {
            var controller = CreateController();
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Proj",
                UserId = "user123",
                ClientId = Guid.NewGuid(),
                TotalHours = 10,
                HourlyRate = 50
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            var dto = new UpdateProjectHoursDto { HoursToAdd = 5 };

            var result = await controller.UpdateProjectHours(project.Id, dto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var summary = Assert.IsType<ProjectHoursSummaryDto>(ok.Value);

            Assert.Equal(15, summary.TotalHours);
        }

        [Fact]
        public async Task GetProjectHoursSummary_ReturnsNotFound()
        {
            var controller = CreateController();
            var result = await controller.GetProjectHoursSummary(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task UploadFileToProject_ReturnsOk()
        {
            var controller = CreateController();

            var proj = new Project
            {
                Id = Guid.NewGuid(),
                Title = "FileProj",
                UserId = "user123",
                ClientId = Guid.NewGuid()
            };

            await _context.Projects.AddAsync(proj);
            await _context.SaveChangesAsync();

            _projectServiceMock.Setup(s => s.GetProjectByIdAsync(proj.Id))
                .ReturnsAsync(new ProjectDto { Id = proj.Id, Title = "FileProj", UserId = "user123" });

            _blobServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("http://fakeurl.com/file.txt");

            var fileMock = new Mock<IFormFile>();
            var content = "test file";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns("file.txt");
            fileMock.Setup(f => f.Length).Returns(stream.Length);

            var result = await controller.UploadFileToProject(proj.Id, fileMock.Object);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(ok.Value);
        }


        [Fact]
        public async Task DeleteProjectFile_ReturnsNoContent()
        {
            var controller = CreateController();

            var file = new freelancer_hub_backend.Models.File
            {
                Id = Guid.NewGuid(),
                FileName = "test",
                FileExtension = ".txt",
                FileUrl = "http://fake/file.txt",
                FileSize = 10
            };

            var projectFile = new ProjectFile
            {
                ProjectId = Guid.NewGuid(),
                FileId = file.Id
            };

            await _context.Files.AddAsync(file);
            await _context.ProjectFiles.AddAsync(projectFile);
            await _context.SaveChangesAsync();

            _blobServiceMock.Setup(s => s.DeleteFileAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await controller.DeleteProjectFile(projectFile.ProjectId, file.Id);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
