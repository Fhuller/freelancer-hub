using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly FreelancerContext _context;

        public PaymentController(FreelancerContext context)
        {
            _context = context;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        {
            var payments = await _context.Payments
                .Select(p => new PaymentDto
                {
                    Id = p.Id,
                    InvoiceId = p.InvoiceId,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMethod,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();

            return Ok(payments);
        }

        // GET: api/payment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetById(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null) return NotFound();

            var dto = new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Notes = payment.Notes,
                CreatedAt = payment.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create(PaymentCreateDto dto)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            var result = new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Notes = payment.Notes,
                CreatedAt = payment.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, result);
        }

        // PUT: api/payment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, PaymentUpdateDto dto)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null) return NotFound();

            payment.Amount = dto.Amount;
            payment.PaymentDate = dto.PaymentDate;
            payment.PaymentMethod = dto.PaymentMethod;
            payment.Notes = dto.Notes;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/payment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);

            if (payment == null) return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
