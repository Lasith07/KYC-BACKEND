using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vue_ts;
using vue_ts.DTOs.Requests;
using vue_ts.Models;
using vue_ts.Services.ImageService;

namespace vue_ts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ApplicationDbContext _context;

        public ImageController(
            IImageService imageService,
            ApplicationDbContext context)
        {
            _imageService = imageService;
            _context = context;
        }

        // Accepts JSON with Base64 strings
        [HttpPost("save")]
        public IActionResult CreateImage([FromBody] CreateImageRequest request)
        {
            var response = _imageService.CreateImage(request);
            return StatusCode(response.status_code, response);
        }

        // Returns the stored Base64 text and its MIME type
        [HttpGet("documents/{id}")]
        public async Task<IActionResult> GetUserDocuments(int id)
        {
            var image = await _context.images.FirstOrDefaultAsync(i => i.id == id);
            if (image == null)
                return NotFound();

            return Ok(new
            {
                NicFrontImage = GetImageResult(image.NicFrontImage),
                NicBackImage = GetImageResult(image.NicBackImage),
                SelfieImage = GetImageResult(image.SelfieImage)
            });
        }

        private object GetImageResult(string base64Data)
        {
            if (string.IsNullOrWhiteSpace(base64Data))
                return null;

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64Data);
            }
            catch
            {
                return null;
            }

            var contentType = DetectImageType(bytes);
            return new
            {
                // Return the raw Base64 (no "data:*" prefix), 
                // frontend can prepend `data:...;base64,` as needed
                Data = base64Data,
                ContentType = contentType
            };
        }

        private string DetectImageType(byte[] imageData)
        {
            if (imageData.Length < 4)
                return "application/octet-stream";

            // JPEG magic numbers
            if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
                return "image/jpeg";

            // PNG magic numbers
            if (imageData[0] == 0x89 && imageData[1] == 0x50 &&
                imageData[2] == 0x4E && imageData[3] == 0x47)
                return "image/png";

            return "application/octet-stream";
        }
    }
}
