using Microsoft.AspNetCore.Mvc;

namespace RestService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private static int _counter = 0;

        [HttpGet]
        public IActionResult Get()
        {
            _counter++;
            if (_counter % 10 == 0)
            {
                return StatusCode(500, "Simulated error.");
            }
            return Ok(new { message = "REST response", time = DateTime.UtcNow });
        }
    }
}
