using freelancer_hub_backend;
using freelancer_hub_backend.Controllers;
using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace freelancer_hub_backend_tests.Controllers
{
    public class InvoiceControllerTests
    {
        private readonly Mock<IUserUtils> _userUtilsMock;

        private FreelancerContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FreelancerContext(options);
        }

        public InvoiceControllerTests()
        {
            _userUtilsMock = new Mock<IUserUtils>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", "test-user-id")
            }));

            _userUtilsMock
                .Setup(u => u.GetJWTUserID(It.IsAny<ClaimsPrincipal>()))
                .Returns("test-user-id");
        }

        private InvoiceController CreateController(FreelancerContext context)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", "test-user-id")
            }));

            return new InvoiceController(context, _userUtilsMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithInvoices()
        {
            var context = GetInMemoryDbContext();

            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = "test-user-id",
                Name = "Cliente A",
                Email = "a@a.com"
            };

            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = "test-user-id",
                ClientId = client.Id,
                Title = "Projeto X",
                DueDate = DateTime.UtcNow.AddDays(10)
            };

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = "test-user-id",
                ClientId = client.Id,
                ProjectId = project.Id,
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(10),
                Amount = 200,
                Status = "paid",
                CreatedAt = DateTime.UtcNow
            };

            context.Clients.Add(client);
            context.Projects.Add(project);
            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            Assert.NotEmpty(list);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var id = Guid.NewGuid();

            var invoice = new Invoice
            {
                Id = id,
                UserId = "test-user-id",
                ClientId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Amount = 500,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.GetById(id);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<InvoiceDto>(ok.Value);

            Assert.Equal(id, dto.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            var controller = CreateController(GetInMemoryDbContext());

            var result = await controller.GetById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var context = GetInMemoryDbContext();
            var controller = CreateController(context);

            var dto = new InvoiceCreateDto
            {
                UserId = "test-user-id",
                ClientId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                Amount = 300,
                Status = "pending",
                PdfUrl = "http://pdf.com"
            };

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdDto = Assert.IsType<InvoiceDto>(created.Value);

            Assert.Equal(dto.Amount, createdDto.Amount);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var id = Guid.NewGuid();

            var invoice = new Invoice
            {
                Id = id,
                UserId = "test-user-id",
                ClientId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                Amount = 250,
                Status = "pending"
            };

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var dto = new InvoiceUpdateDto
            {
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate.AddDays(2),
                Amount = 400,
                Status = "paid",
                PdfUrl = "x"
            };

            var result = await controller.Update(id, dto);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(400, invoice.Amount);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenNotExists()
        {
            var controller = CreateController(GetInMemoryDbContext());

            var dto = new InvoiceUpdateDto
            {
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(5),
                Amount = 100,
                Status = "pending",
                PdfUrl = "x"
            };

            var result = await controller.Update(Guid.NewGuid(), dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenExists()
        {
            var context = GetInMemoryDbContext();
            var id = Guid.NewGuid();

            var invoice = new Invoice
            {
                Id = id,
                UserId = "test-user-id",
                ClientId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid()
            };

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            var controller = CreateController(GetInMemoryDbContext());

            var result = await controller.Delete(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
