using System;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;
namespace vue_ts.Services.DetailService
{
	public interface IDetailService
	{
        BaseResponse CreateDetail(CreateDetailRequest request);
    }
}

