
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using vue_ts;
using vue_ts.DTOs.Requests;
using vue_ts.Models;
using vue_ts.Services;
using vue_ts.DTOs;
using vue_ts.DTOs.Responses;

namespace vue_ts.Services
{
    public class OtpService : IOtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISmsService _smsService;
        private readonly ShoutOutSmsConfiguration _smsConfig;

        public OtpService(ApplicationDbContext context, ISmsService smsService, IOptions<ShoutOutSmsConfiguration> options)
        {
            _context = context;
            _smsService = smsService;
            _smsConfig = options.Value;
        }

        public BaseResponse SendAndStoreOtp(OtpRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.PhoneNumber))
                {
                    return new BaseResponse { Success = false, Message = "Phone number is required." };
                }

                // Generate OTP
                var otpCode = new Random().Next(100000, 999999).ToString();
                var expiresAt = DateTimeOffset.UtcNow.AddMinutes(5);

                // Send OTP via SMS
                bool smsSuccess = _smsService.SendOtpAsync(request.PhoneNumber, otpCode).GetAwaiter().GetResult();

                if (!smsSuccess)
                {
                    return new BaseResponse { Success = false, Message = "Failed to send OTP." };
                }

                // Check for existing OTP record
                var existingOtp = _context.Otps.FirstOrDefault(o => o.PhoneNumber == request.PhoneNumber);

                if (existingOtp != null)
                {
                    // Update existing record
                    existingOtp.OtpCode = otpCode;
                    existingOtp.ExpiresAt = expiresAt;
                    existingOtp.IsUsed = false;
                }
                else
                {
                    // Create new record
                    var otp = new Otp
                    {
                        PhoneNumber = request.PhoneNumber,
                        OtpCode = otpCode,
                        ExpiresAt = expiresAt,
                        IsUsed = false
                    };
                    _context.Otps.Add(otp);
                }

                // Save changes to database
                _context.SaveChanges();

                return new BaseResponse { Success = true, Message = "OTP sent successfully." };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Success = false, Message = $"Error sending OTP: {ex.Message}" };
            }
        }

        public BaseResponse VerifyOtp(OtpVerificationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.OtpCode))
                {
                    return new BaseResponse { Success = false, Message = "Phone number and OTP are required." };
                }

                var otp = _context.Otps
                    .Where(o => o.PhoneNumber == request.PhoneNumber &&
                                o.OtpCode == request.OtpCode &&
                                !o.IsUsed &&
                                o.ExpiresAt > DateTimeOffset.UtcNow)
                    .FirstOrDefault();

                if (otp == null)
                {
                    return new BaseResponse { Success = false, Message = "Invalid or expired OTP." };
                }

                otp.IsUsed = true;
                _context.SaveChanges();

                return new BaseResponse { Success = true, Message = "OTP verified successfully." };
            }
            catch (Exception ex)
            {
                return new BaseResponse { Success = false, Message = $"Error verifying OTP: {ex.Message}" };
            }
        }
    }
}