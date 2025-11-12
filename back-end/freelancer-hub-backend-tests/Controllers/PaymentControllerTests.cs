using freelancer_hub_backend;
using freelancer_hub_backend.Controllers;
using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend_tests.Controllers
{
    public class PaymentControllerTests : IDisposable
    {
        private readonly FreelancerContext _context;
        private readonly PaymentController _controller;

        public PaymentControllerTests()
        {
            var options = new DbContextOptionsBuilder<FreelancerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FreelancerContext(options);
            _context.Database.EnsureCreated();
            _controller = new PaymentController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithListOfPayments()
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = "user-1",
                InvoiceId = Guid.NewGuid(),
                Amount = 100.5m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "PIX",
                Notes = "Test note",
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<PaymentDto>>(ok.Value);

            var dto = list.First();
            Assert.Equal(payment.Id, dto.Id);
            Assert.Equal(payment.InvoiceId, dto.InvoiceId);
            Assert.Equal(payment.Amount, dto.Amount);
            Assert.Equal(payment.PaymentMethod, dto.PaymentMethod);
            Assert.Equal(payment.Notes, dto.Notes);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenFound()
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = "user-1",
                InvoiceId = Guid.NewGuid(),
                Amount = 500m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Credit",
                Notes = "Invoice paid",
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(payment.Id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PaymentDto>(ok.Value);

            Assert.Equal(payment.Id, dto.Id);
            Assert.Equal(payment.Amount, dto.Amount);
            Assert.Equal(payment.PaymentMethod, dto.PaymentMethod);
            Assert.Equal(payment.Notes, dto.Notes);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.GetById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WithValidDto()
        {
            var dto = new PaymentCreateDto
            {
                UserId = "user-2",
                InvoiceId = Guid.NewGuid(),
                Amount = 250.75m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Debit",
                Notes = "Partial payment"
            };

            var result = await _controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var payment = Assert.IsType<PaymentDto>(created.Value);

            var entity = await _context.Payments.FindAsync(payment.Id);
            Assert.NotNull(entity);

            Assert.Equal(dto.InvoiceId, entity.InvoiceId);
            Assert.Equal(dto.Amount, entity.Amount);
            Assert.Equal(dto.PaymentMethod, entity.PaymentMethod);
            Assert.Equal(dto.Notes, entity.Notes);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSuccessful()
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = "user-3",
                InvoiceId = Guid.NewGuid(),
                Amount = 100,
                PaymentDate = DateTime.UtcNow.AddDays(-1),
                PaymentMethod = "PIX",
                Notes = "Old",
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var updateDto = new PaymentUpdateDto
            {
                Amount = 200,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Credit",
                Notes = "Updated"
            };

            var result = await _controller.Update(payment.Id, updateDto);
            Assert.IsType<NoContentResult>(result);

            var updated = await _context.Payments.FindAsync(payment.Id);
            Assert.Equal(200, updated.Amount);
            Assert.Equal("Credit", updated.PaymentMethod);
            Assert.Equal("Updated", updated.Notes);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenIdDoesNotExist()
        {
            var updateDto = new PaymentUpdateDto
            {
                Amount = 300,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "PIX",
                Notes = "Try update"
            };

            var result = await _controller.Update(Guid.NewGuid(), updateDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenFound()
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = "user-4",
                InvoiceId = Guid.NewGuid(),
                Amount = 400,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "Cash",
                Notes = "Delete me",
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(payment.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.Payments.FindAsync(payment.Id));
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenDoesNotExist()
        {
            var result = await _controller.Delete(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
