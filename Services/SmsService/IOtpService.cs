using vue_ts.DTOs;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;

namespace vue_ts.Services
{
    public interface IOtpService
    {
        BaseResponse SendAndStoreOtp(OtpRequest request);
        BaseResponse VerifyOtp(OtpVerificationRequest request);
    }
}