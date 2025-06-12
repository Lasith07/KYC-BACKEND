
using Microsoft.AspNetCore.Mvc;
using vue_ts.DTOs;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;
using vue_ts.Services;

namespace vue_ts.Controllers
{
    [Route("api/Otp")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send")]
        public BaseResponse SendOtp(OtpRequest request)
        {
            return _otpService.SendAndStoreOtp(request);
        }

        [HttpPost("verify")]
        public BaseResponse VerifyOtp(OtpVerificationRequest request)
        {
            return _otpService.VerifyOtp(request);
        }

        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return "otp work";
        }
    }
}