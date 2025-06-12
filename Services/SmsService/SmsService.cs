using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using vue_ts.DTOs.Requests;
using vue_ts.DTOs.Responses;
using vue_ts.Models;

namespace vue_ts.Services
{
    public class SmsService : ISmsService
    {
        private readonly HttpClient _httpClient;
        private readonly ShoutOutSmsConfiguration _smsConfig;
        private readonly ILogger<SmsService> _logger;

        public SmsService(
            HttpClient httpClient,
            IOptions<ShoutOutSmsConfiguration> smsConfig,
            ILogger<SmsService> logger)
        {
            _httpClient = httpClient;
            _smsConfig = smsConfig.Value;
            _logger = logger;

            // Validate configuration
            if (string.IsNullOrEmpty(_smsConfig.Token))
            {
                throw new ArgumentNullException("ShoutOut token is missing in configuration");
            }
        }

        public async Task<bool> SendOtpAsync(string phoneNumber, string otpCode)
        {
            try
            {
                if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(otpCode))
                {
                    _logger.LogError("Invalid phone number or OTP code.");
                    return false;
                }

                var formattedPhone = FormatPhoneNumber(phoneNumber);
                var message = $"Your OTP for {_smsConfig.Mask} is {otpCode}. Valid for 5 minutes.";

                var response = await SendShoutOutSms(formattedPhone, message);
                return response.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendOtpAsync error: {ex.Message}");
                return false;
            }
        }

        private async Task<BaseResponse> SendShoutOutSms(string phoneNumber, string message)
        {
            try
            {
                // Construct ShoutOut API payload
                var requestBody = new
                {
                    source = _smsConfig.Mask,
                    destinations = new[] { phoneNumber },
                    content = new { sms = message },
                    transports = new[] { "sms" }
                };

                // authorization header
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _smsConfig.Token);

                var content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(_smsConfig.SmsAPI, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"ShoutOut API failed: {response.StatusCode} - {responseContent}");
                    return new BaseResponse
                    {
                        Success = false,
                        status_code = (int)response.StatusCode,
                        Message = $"API Error: {response.StatusCode}"
                    };
                }

                // Parse ShoutOut response
                var apiResponse = JsonConvert.DeserializeObject<ShoutOutResponse>(responseContent);
                if (apiResponse?.Status == "1001")  // Success code
                {
                    return new BaseResponse
                    {
                        Success = true,
                        status_code = 200,
                        Message = "SMS sent successfully"
                    };
                }

                return new BaseResponse
                {
                    Success = false,
                    status_code = 500,
                    Message = apiResponse?.Message ?? "Unknown API error"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendShoutOutSms error: {ex.Message}");
                return new BaseResponse
                {
                    Success = false,
                    status_code = 500,
                    Message = ex.Message
                };
            }
        }

        public async Task<BaseResponse> SendSmsAsync(SmsDetailsDTO details)
        {
            try
            {
                if (string.IsNullOrEmpty(details.PhoneNumber) || string.IsNullOrEmpty(details.Message))
                {
                    _logger.LogError("Invalid phone number or message.");
                    return new BaseResponse
                    {
                        Success = false,
                        status_code = 400,
                        Message = "Invalid phone number or message"
                    };
                }

                var formattedPhone = FormatPhoneNumber(details.PhoneNumber);
                return await SendShoutOutSms(formattedPhone, details.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendSmsAsync error: {ex.Message}");
                return new BaseResponse
                {
                    Success = false,
                    status_code = 500,
                    Message = ex.Message
                };
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            
            var cleaned = new StringBuilder();
            foreach (char c in phoneNumber)
            {
                if (char.IsDigit(c) || (c == '+' && cleaned.Length == 0))
                {
                    cleaned.Append(c);
                }
            }

            
            string result = cleaned.ToString();
            return result.StartsWith("+") ? result[1..] : result;
        }
    }

    
    public class ShoutOutResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }  // "1001" = success

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}