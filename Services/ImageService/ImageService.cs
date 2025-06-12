using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.Logging;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;
using vue_ts.Models;

namespace vue_ts.Services.ImageService
{
    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImageService> _logger;

        public ImageService(ApplicationDbContext context, ILogger<ImageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public BaseResponse CreateImage(CreateImageRequest request)
        {
            try
            {
                // 1) Validate request
                var ctx = new ValidationContext(request);
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(request, ctx, results, true))
                {
                    var errors = results.Select(r => r.ErrorMessage).ToList();
                    return new BaseResponse(false, 400, new { message = "Validation failed", errors });
                }

                // 2) Strip any data URI prefix (e.g. "data:image/png;base64,")
                string Clean(string s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return s;
                    var parts = s.Split(',');
                    return parts.Length > 1 ? parts[1] : parts[0];
                }

                // 3) Create model with Base64 strings
                var model = new ImageModel
                {
                    customerid = Guid.NewGuid(),
                    id = request.id,
                    SelfieImage = Clean(request.SelfieImage),
                    NicFrontImage = Clean(request.NicFrontImage),
                    NicBackImage = Clean(request.NicBackImage)
                };

                // 4) Persist
                _context.images.Add(model);
                _context.SaveChanges();

                return new BaseResponse(true, 200, new { message = "Images saved as Base64 text" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving images as Base64 text");
                return new BaseResponse(false, 500, new { message = "Internal server error: " + ex.Message });
            }
        }
    }
}
