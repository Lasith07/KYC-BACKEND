using System;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;


namespace vue_ts.Services.ImageService
{
	public interface IImageService
	{
        BaseResponse CreateImage(CreateImageRequest request);
    }
}

