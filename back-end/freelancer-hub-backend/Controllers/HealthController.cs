using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly FreelancerContext db;

        public HealthController(FreelancerContext context)
        {
            db = context;
        }


        [HttpGet, HttpHead]
        public async Task<IActionResult> Get()
        {
            try
            {
                await db.Database.ExecuteSqlRawAsync("SELECT 1");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HealthCheck] Falha no banco: {ex.Message}");
            }

            return Ok("OK");
        }
    }
}
