using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace vue_ts.Models
{
    [Table("images")]
    public class ImageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid customerid { get; set; }

        
        [ForeignKey("customer")] 
        public int id { get; set; }  

        
        public virtual DetailModel customer { get; set; }

        public string SelfiePath { get; set; }
        public string NicFrontPath { get; set; }
        public string NicBackPath { get; set; }

        
    }
}