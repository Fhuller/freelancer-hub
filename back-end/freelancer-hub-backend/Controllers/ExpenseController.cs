using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly FreelancerContext _context;

        public ExpenseController(FreelancerContext context)
        {
            _context = context;
        }

        // GET: api/expense
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetAll()
        {
            var expenses = await _context.Expenses
                .Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Title = e.Title,
                    Amount = e.Amount,
                    Category = e.Category,
                    PaymentDate = e.PaymentDate,
                    Notes = e.Notes,
                    CreatedAt = e.CreatedAt
                })
                .ToListAsync();

            return Ok(expenses);
        }

        // GET: api/expense/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpenseDto>> GetById(Guid id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null) return NotFound();

            var dto = new ExpenseDto
            {
                Id = expense.Id,
                UserId = expense.UserId,
                Title = expense.Title,
                Amount = expense.Amount,
                Category = expense.Category,
                PaymentDate = expense.PaymentDate,
                Notes = expense.Notes,
                CreatedAt = expense.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/expense
        [HttpPost]
        public async Task<ActionResult<ExpenseDto>> Create(ExpenseCreateDto dto)
        {
            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Title = dto.Title,
                Amount = dto.Amount,
                Category = dto.Category,
                PaymentDate = dto.PaymentDate,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            var result = new ExpenseDto
            {
                Id = expense.Id,
                UserId = expense.UserId,
                Title = expense.Title,
                Amount = expense.Amount,
                Category = expense.Category,
                PaymentDate = expense.PaymentDate,
                Notes = expense.Notes,
                CreatedAt = expense.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = expense.Id }, result);
        }

        // PUT: api/expense/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, ExpenseUpdateDto dto)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null) return NotFound();

            expense.Title = dto.Title;
            expense.Amount = dto.Amount;
            expense.Category = dto.Category;
            expense.PaymentDate = dto.PaymentDate;
            expense.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/expense/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null) return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
