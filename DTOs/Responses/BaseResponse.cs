namespace vue_ts.DTOs.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public int status_code { get; set; }
        public object? data { get; set; }

        public string Message { get; set; }

        public BaseResponse() { }

        public BaseResponse(bool success, int status_code, object? data)
        {
            Success = success;
            this.status_code = status_code;
            this.data = data; // Allow null data for error cases
        }
    }
}