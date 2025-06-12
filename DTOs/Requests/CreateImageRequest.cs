using System.ComponentModel.DataAnnotations;

namespace vue_ts.DTOs.Requests
{
    public class CreateImageRequest
    {
        [Required]
        public string SelfieImage { get; set; }      // base64 string

        [Required]
        public string NicFrontImage { get; set; }    // base64 string

        [Required]
        public string NicBackImage { get; set; }     // base64 string

        [Required]
        public int id { get; set; }
    }
}
