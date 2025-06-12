using System.ComponentModel.DataAnnotations;

namespace vue_ts.DTOs.Requests

{

public class CreateDetailRequest

    {

        [Required]

        public int id { get; set; }

        [Required]


        public string title { get; set; }

        [Required]



        public string fullName { get; set; }

        [Required]



        public long mobileNumber { get; set; }

        [Required]



        public string email { get; set; }

        [Required]


        public string nicNumber { get; set; }


        [Required]

        public string nationality { get; set; }


    }
}