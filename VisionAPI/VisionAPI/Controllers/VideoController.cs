using Microsoft.AspNetCore.Mvc;
using VisionAPI.Services;

namespace VisionAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase
    {
        private readonly ImageScrapperService _scrapperService;
        public VideoController(ImageScrapperService imageScrapperService)
        {
            _scrapperService = imageScrapperService;
        }

        [HttpPost]
        public async Task<IActionResult> RunService()
        {
            _scrapperService.Run();

            return Ok();
        }
    }
}
