using Microsoft.AspNetCore.Mvc;

namespace VisionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase
    {
        public VideoController()
        {
        }

        [HttpGet]
        public async Task<IActionResult> IsCamHealthy()
        {
            return Ok(true);
        }
    }
}
