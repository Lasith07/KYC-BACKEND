using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using vue_ts.Models;


namespace vue_ts.Models
{
    [Table("customer")]
    public class DetailModel

    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int id { get; set; }

        [Required]

        public string title { get; set; }

        [Required]

        public string fullName { get; set; }

        [Required]

        public long mobileNumber { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string nicNumber { get; set; }

        [Required]
        public string nationality { get; set; }

        [Required]


        public virtual ImageModel images { get; set; }
    }
}
    
    