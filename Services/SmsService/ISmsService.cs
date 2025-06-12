
namespace vue_ts.Services
{
    public interface ISmsService
    {
        Task<bool> SendOtpAsync(string PhoneNumber, string otpCode);
    }
}
