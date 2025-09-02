using Microsoft.AspNetCore.Mvc;

namespace freelancer_hub_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet, HttpHead]
        public ActionResult Get()
        {
            return Ok("OK");
        }
    }
}
