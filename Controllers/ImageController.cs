using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vue_ts.Services.ImageService;
using vue_ts.DTOs.Requests;

namespace vue_ts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromForm] CreateImageRequest request)
        {
            var response = await _imageService.SaveImagesAsync(request);
            return base.StatusCode(response.StatusCode, response.Data);
        }

        [HttpGet("documents/{id}")]
        public async Task<IActionResult> GetUserDocuments(int id)
        {
            var response = await _imageService.GetUserDocumentsAsync(id);
            return base.StatusCode(response.StatusCode, response.Data);
        }
    }
}