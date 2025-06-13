using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using vue_ts.DTOs.Requests;
using vue_ts.Models;
using vue_ts.DTOs.Responses;

namespace vue_ts.Services.ImageService
{
    public class ImageService : IImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImageService> _logger;
        private readonly string _assetsPath;

        public ImageService(ApplicationDbContext context, ILogger<ImageService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _assetsPath = Path.Combine(Directory.GetCurrentDirectory(), "assets");
            Directory.CreateDirectory(_assetsPath);
        }

        public async Task<ServiceResponse> SaveImagesAsync(CreateImageRequest request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Received null CreateImageRequest");
                    return new ServiceResponse(false, StatusCodes.Status400BadRequest, new { Message = "Request cannot be null" });
                }

                var customer = await _context.Customers.FindAsync(request.Id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {Id} not found", request.Id);
                    return new ServiceResponse(false, StatusCodes.Status404NotFound, new { Message = "Customer not found" });
                }

                var errors = ValidateImages(request);
                if (errors.Any())
                {
                    _logger.LogWarning("Image validation failed: {Errors}", string.Join("; ", errors));
                    return new ServiceResponse(false, StatusCodes.Status400BadRequest, new { Message = "Invalid images", Errors = errors });
                }

                var image = await _context.images.FirstOrDefaultAsync(i => i.id == request.Id)
                           ?? new ImageModel { id = request.Id, customer = customer };

                if (request.NicFrontImage != null)
                {
                    DeleteExistingFile(image.NicFrontPath);
                    image.NicFrontPath = await SaveImageAsync(request.NicFrontImage, "nic_front");
                }
                if (request.NicBackImage != null)
                {
                    DeleteExistingFile(image.NicBackPath);
                    image.NicBackPath = await SaveImageAsync(request.NicBackImage, "nic_back");
                }
                if (request.SelfieImage != null)
                {
                    DeleteExistingFile(image.SelfiePath);
                    image.SelfiePath = await SaveImageAsync(request.SelfieImage, "selfie");
                }

                if (_context.Entry(image).State == EntityState.Detached)
                    _context.images.Add(image);
                else
                    _context.images.Update(image);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Saved images for customer ID {CustomerId}", image.id);
                return new ServiceResponse(true, StatusCodes.Status201Created, new
                {
                    Message = "Images saved successfully",
                    CustomerId = image.id,
                    ImageRecordId = image.customerid,
                    Paths = new
                    {
                        NicFront = image.NicFrontPath,
                        NicBack = image.NicBackPath,
                        Selfie = image.SelfiePath
                    }
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving images for customer ID {Id}", request?.Id);
                return new ServiceResponse(false, StatusCodes.Status400BadRequest, new { Message = "Failed to save images due to invalid data" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving images for customer ID {Id}", request?.Id);
                return new ServiceResponse(false, StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred" });
            }
        }

        private List<string> ValidateImages(CreateImageRequest request)
        {
            var errors = new List<string>();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            const long maxFileSize = 2 * 1024 * 1024; // 2MB to match frontend

            if (request.NicFrontImage != null && !IsValidImage(request.NicFrontImage, allowedExtensions, maxFileSize))
                errors.Add("Invalid NIC front image (must be JPG/PNG, max 2MB)");
            if (request.NicBackImage != null && !IsValidImage(request.NicBackImage, allowedExtensions, maxFileSize))
                errors.Add("Invalid NIC back image (must be JPG/PNG, max 2MB)");
            if (request.SelfieImage != null && !IsValidImage(request.SelfieImage, allowedExtensions, maxFileSize))
                errors.Add("Invalid selfie image (must be JPG/PNG, max 2MB)");
            if (request.NicFrontImage == null && request.NicBackImage == null && request.SelfieImage == null)
                errors.Add("At least one image is required");

            return errors;
        }

        private bool IsValidImage(IFormFile file, string[] allowedExtensions, long maxFileSize)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return file.Length <= maxFileSize && allowedExtensions.Contains(extension);
        }

        private async Task<string> SaveImageAsync(IFormFile file, string prefix)
        {
            var fileName = $"{prefix}_{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            var filePath = Path.Combine(_assetsPath, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"assets/{fileName}";
        }

        private void DeleteExistingFile(string? filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                if (File.Exists(fullPath))
                {
                    try
                    {
                        File.Delete(fullPath);
                        _logger.LogInformation("Deleted old image file: {FilePath}", filePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete old image file: {FilePath}", filePath);
                    }
                }
            }
        }

        public Task<ServiceResponse> GetUserDocumentsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

}
    