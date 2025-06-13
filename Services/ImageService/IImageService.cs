using System.Threading.Tasks;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;


namespace vue_ts.Services.ImageService
{
    public interface IImageService
    {
        Task<ServiceResponse> SaveImagesAsync(CreateImageRequest request);
        Task<ServiceResponse> GetUserDocumentsAsync(int id);
    }

    
} 