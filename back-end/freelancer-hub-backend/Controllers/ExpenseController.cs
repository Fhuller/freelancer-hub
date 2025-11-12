using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Services;
using freelancer_hub_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace freelancer_hub_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IUserUtils _userUtils;
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService, IUserUtils userUtils)
        {
            _expenseService = expenseService;
            _userUtils = userUtils;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
        {
            try
            {
                var userId = _userUtils.GetJWTUserID(User);
                var expenses = await _expenseService.GetExpensesAsync(userId);
                return Ok(expenses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetExpenseById(Guid id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null) return NotFound();

            return Ok(expense);
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseDto>> CreateExpense(ExpenseCreateDto dto)
        {
            try
            {
                var userId = _userUtils.GetJWTUserID(User);
                var expense = await _expenseService.CreateExpenseAsync(userId, dto);
                return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expense);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(Guid id, ExpenseUpdateDto dto)
        {
            try
            {
                await _expenseService.UpdateExpenseAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            try
            {
                await _expenseService.DeleteExpenseAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}