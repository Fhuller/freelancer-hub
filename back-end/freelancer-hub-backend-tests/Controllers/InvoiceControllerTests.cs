using freelancer_hub_backend;
using freelancer_hub_backend.Controllers;
using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend_tests.Controllers
{
    public class InvoiceControllerTests
    {
        private FreelancerContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new FreelancerContext(options);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithInvoices()
        {
            var context = GetInMemoryDbContext();

            var client = new Client
            {
                Id = Guid.NewGuid(),
                UserId = "test-user",
                Name = "Cliente A",
                Email = "a@a.com"
            };

            var project = new Project
            {
                Id = Guid.NewGuid(),
                UserId = "test-user",
                ClientId = client.Id,
                Title = "Projeto X",
                DueDate = DateTime.UtcNow.AddDays(10)
            };

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = "test-user",
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

            var controller = new InvoiceController(context);

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
                UserId = "test",
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

            var controller = new InvoiceController(context);

            var result = await controller.GetById(id);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<InvoiceDto>(ok.Value);

            Assert.Equal(id, dto.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            var controller = new InvoiceController(GetInMemoryDbContext());

            var result = await controller.GetById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreated()
        {
            var context = GetInMemoryDbContext();
            var controller = new InvoiceController(context);

            var dto = new InvoiceCreateDto
            {
                UserId = "user",
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
                UserId = "user",
                ClientId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid(),
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                Amount = 250,
                Status = "pending"
            };

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var controller = new InvoiceController(context);

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
            var controller = new InvoiceController(GetInMemoryDbContext());

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
                UserId = "user",
                ClientId = Guid.NewGuid(),
                ProjectId = Guid.NewGuid()
            };

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var controller = new InvoiceController(context);
            var result = await controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenNotExists()
        {
            var controller = new InvoiceController(GetInMemoryDbContext());

            var result = await controller.Delete(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
