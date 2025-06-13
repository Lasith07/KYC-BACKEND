using System;
namespace vue_ts.DTOs.Responses
{
    public class ServiceResponse
    {
        internal object status_code;

        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }

        public ServiceResponse(bool success, int statusCode, object data)
        {
            Success = success;
            StatusCode = statusCode;
            Data = data;
        }
    }
}

