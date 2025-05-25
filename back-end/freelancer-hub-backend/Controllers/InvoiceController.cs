using freelancer_hub_backend.DTO_s;
using freelancer_hub_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly FreelancerContext _context;

        public InvoiceController(FreelancerContext context)
        {
            _context = context;
        }

        // GET: api/invoice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
        {
            var invoices = await _context.Invoices
                .Select(i => new InvoiceDto
                {
                    Id = i.Id,
                    UserId = i.UserId,
                    ClientId = i.ClientId,
                    ProjectId = i.ProjectId,
                    IssueDate = i.IssueDate,
                    DueDate = i.DueDate,
                    Amount = i.Amount,
                    Status = i.Status,
                    PdfUrl = i.PdfUrl,
                    CreatedAt = i.CreatedAt
                })
                .ToListAsync();

            return Ok(invoices);
        }

        // GET: api/invoice/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null) return NotFound();

            var dto = new InvoiceDto
            {
                Id = invoice.Id,
                UserId = invoice.UserId,
                ClientId = invoice.ClientId,
                ProjectId = invoice.ProjectId,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                Amount = invoice.Amount,
                Status = invoice.Status,
                PdfUrl = invoice.PdfUrl,
                CreatedAt = invoice.CreatedAt
            };

            return Ok(dto);
        }

        // POST: api/invoice
        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> Create(InvoiceCreateDto dto)
        {
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ClientId = dto.ClientId,
                ProjectId = dto.ProjectId,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                Amount = dto.Amount,
                Status = dto.Status,
                PdfUrl = dto.PdfUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            var result = new InvoiceDto
            {
                Id = invoice.Id,
                UserId = invoice.UserId,
                ClientId = invoice.ClientId,
                ProjectId = invoice.ProjectId,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                Amount = invoice.Amount,
                Status = invoice.Status,
                PdfUrl = invoice.PdfUrl,
                CreatedAt = invoice.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, result);
        }

        // PUT: api/invoice/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, InvoiceUpdateDto dto)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null) return NotFound();

            invoice.IssueDate = dto.IssueDate;
            invoice.DueDate = dto.DueDate;
            invoice.Amount = dto.Amount;
            invoice.Status = dto.Status;
            invoice.PdfUrl = dto.PdfUrl;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/invoice/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null) return NotFound();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
