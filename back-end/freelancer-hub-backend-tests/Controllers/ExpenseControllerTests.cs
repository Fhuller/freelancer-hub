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
    public class ExpenseControllerTests
    {
        private readonly Mock<IExpenseService> _expenseServiceMock;
        private readonly Mock<IUserUtils> _userUtilsMock;
        private readonly ExpenseController _controller;

        public ExpenseControllerTests()
        {
            _expenseServiceMock = new Mock<IExpenseService>();
            _userUtilsMock = new Mock<IUserUtils>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", "test-user-id")
            }));

            _userUtilsMock.Setup(u => u.GetSupabaseUserId(user)).Returns("test-user-id");

            _controller = new ExpenseController(_expenseServiceMock.Object, _userUtilsMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        [Fact]
        public async Task GetExpenses_ReturnsOk_WithValidDtos()
        {
            var expenses = new List<ExpenseDto>
            {
                new ExpenseDto
                {
                    Id = Guid.NewGuid(),
                    UserId = "test-user-id",
                    Title = "Expense 1",
                    Amount = 123.45m,
                    Category = "Food",
                    PaymentDate = DateTime.UtcNow,
                    Notes = "Lunch",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _expenseServiceMock.Setup(s => s.GetExpensesAsync("test-user-id"))
                               .ReturnsAsync(expenses);

            var result = await _controller.GetExpenses();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsAssignableFrom<IEnumerable<ExpenseDto>>(ok.Value);
            var expense = value.First();

            Assert.Equal("test-user-id", expense.UserId);
            Assert.Equal("Expense 1", expense.Title);
            Assert.Equal("Food", expense.Category);
            Assert.Equal("Lunch", expense.Notes);
            Assert.Equal(123.45m, expense.Amount);
        }

        [Fact]
        public async Task GetExpenses_ReturnsUnauthorized_WhenException()
        {
            _expenseServiceMock.Setup(s => s.GetExpensesAsync("test-user-id"))
                               .ThrowsAsync(new UnauthorizedAccessException("no access"));

            var result = await _controller.GetExpenses();

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var json = System.Text.Json.JsonSerializer.Serialize(unauthorized.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("no access", dict["message"]);
        }

        [Fact]
        public async Task GetExpenseById_ReturnsOk_WithValidDto()
        {
            var id = Guid.NewGuid();
            var dto = new ExpenseDto
            {
                Id = id,
                UserId = "test-user-id",
                Title = "Rent",
                Amount = 800,
                Category = "Housing",
                PaymentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _expenseServiceMock.Setup(s => s.GetExpenseByIdAsync(id))
                               .ReturnsAsync(dto);

            var result = await _controller.GetExpenseById(id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var expense = Assert.IsType<ExpenseDto>(ok.Value);

            Assert.Equal(id, expense.Id);
            Assert.Equal("Rent", expense.Title);
            Assert.Equal("Housing", expense.Category);
            Assert.Equal(800, expense.Amount);
        }

        [Fact]
        public async Task GetExpenseById_ReturnsNotFound_WhenNull()
        {
            _expenseServiceMock.Setup(s => s.GetExpenseByIdAsync(It.IsAny<Guid>()))
                               .ReturnsAsync((ExpenseDto)null);

            var result = await _controller.GetExpenseById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateExpense_ReturnsCreated_WithValidDto()
        {
            var createDto = new ExpenseCreateDto
            {
                UserId = "test-user-id",
                Title = "Gym Membership",
                Amount = 150.75m,
                Category = "Health",
                PaymentDate = new DateTime(2025, 1, 1),
                Notes = "Monthly subscription"
            };

            var createdDto = new ExpenseDto
            {
                Id = Guid.NewGuid(),
                UserId = "test-user-id",
                Title = createDto.Title,
                Amount = createDto.Amount,
                Category = createDto.Category,
                PaymentDate = createDto.PaymentDate,
                Notes = createDto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _expenseServiceMock.Setup(s => s.CreateExpenseAsync("test-user-id", createDto))
                               .ReturnsAsync(createdDto);

            var result = await _controller.CreateExpense(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var dto = Assert.IsType<ExpenseDto>(createdResult.Value);

            Assert.Equal(createDto.UserId, dto.UserId);
            Assert.Equal(createDto.Title, dto.Title);
            Assert.Equal(createDto.Amount, dto.Amount);
            Assert.Equal(createDto.Category, dto.Category);
            Assert.Equal(createDto.PaymentDate, dto.PaymentDate);
            Assert.Equal(createDto.Notes, dto.Notes);
        }

        [Fact]
        public async Task CreateExpense_ReturnsUnauthorized_WhenUnauthorized()
        {
            var dto = new ExpenseCreateDto
            {
                UserId = "test-user-id",
                Title = "Unauthorized",
                Amount = 0,
                Category = "Other",
                PaymentDate = DateTime.UtcNow
            };

            _expenseServiceMock.Setup(s => s.CreateExpenseAsync("test-user-id", dto))
                               .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.CreateExpense(dto);

            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task CreateExpense_ReturnsBadRequest_WhenInvalidArgument()
        {
            var dto = new ExpenseCreateDto
            {
                UserId = "test-user-id",
                Title = "Bad",
                Amount = 0,
                Category = "Other",
                PaymentDate = DateTime.UtcNow
            };

            _expenseServiceMock.Setup(s => s.CreateExpenseAsync("test-user-id", dto))
                               .ThrowsAsync(new ArgumentException("invalid"));

            var result = await _controller.CreateExpense(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            var json = System.Text.Json.JsonSerializer.Serialize(bad.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("invalid", dict["message"]);
        }

        [Fact]
        public async Task UpdateExpense_ReturnsNoContent()
        {
            var updateDto = new ExpenseUpdateDto
            {
                Title = "Updated Title",
                Amount = 200,
                Category = "Updated",
                PaymentDate = DateTime.UtcNow,
                Notes = "Updated notes"
            };

            _expenseServiceMock.Setup(s => s.UpdateExpenseAsync(It.IsAny<Guid>(), updateDto))
                               .Returns(Task.CompletedTask);

            var result = await _controller.UpdateExpense(Guid.NewGuid(), updateDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateExpense_ReturnsNotFound_WhenKeyNotFound()
        {
            _expenseServiceMock.Setup(s => s.UpdateExpenseAsync(It.IsAny<Guid>(), It.IsAny<ExpenseUpdateDto>()))
                               .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateExpense(Guid.NewGuid(), new ExpenseUpdateDto());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateExpense_ReturnsBadRequest_WhenArgumentException()
        {
            _expenseServiceMock.Setup(s => s.UpdateExpenseAsync(It.IsAny<Guid>(), It.IsAny<ExpenseUpdateDto>()))
                               .ThrowsAsync(new ArgumentException("error"));

            var result = await _controller.UpdateExpense(Guid.NewGuid(), new ExpenseUpdateDto());

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var json = System.Text.Json.JsonSerializer.Serialize(bad.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            Assert.Equal("error", dict["message"]);
        }

        [Fact]
        public async Task DeleteExpense_ReturnsNoContent()
        {
            _expenseServiceMock.Setup(s => s.DeleteExpenseAsync(It.IsAny<Guid>()))
                               .Returns(Task.CompletedTask);

            var result = await _controller.DeleteExpense(Guid.NewGuid());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteExpense_ReturnsNotFound_WhenKeyNotFound()
        {
            _expenseServiceMock.Setup(s => s.DeleteExpenseAsync(It.IsAny<Guid>()))
                               .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeleteExpense(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
