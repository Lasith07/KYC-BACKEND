namespace vue_ts.DTOs.Responses
{
    
    public class MessageDTO
    {
        
        public bool Success { get; set; }

        
        public string Message { get; set; }

        
        public MessageDTO() { }

        
        public MessageDTO(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        
        public MessageDTO(string message) : this(true, message) { }
    }
}
