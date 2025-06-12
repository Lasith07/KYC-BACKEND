using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vue_ts.Models
{
    [Table ("images")]
	public class ImageModel
	{

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid customerid { get; set; }

        [ForeignKey("id")]
        public int id { get; set; } 

        public string SelfieImage { get; set; }
        
        public string NicFrontImage { get; set; }

        public string NicBackImage { get; set; }

       
    }
}


