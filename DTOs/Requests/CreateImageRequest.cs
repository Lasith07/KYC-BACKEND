using System.ComponentModel.DataAnnotations;


namespace vue_ts.DTOs.Requests
{
    public class CreateImageRequest
    {
        [Required]
        public int Id { get; set; }
        public IFormFile? NicFrontImage { get; set; }
        public IFormFile? NicBackImage { get; set; }
        public IFormFile? SelfieImage { get; set; }
    }
}
