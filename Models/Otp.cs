
using System;
using System.ComponentModel.DataAnnotations;

namespace vue_ts.Models
{
    public class Otp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(6)]
        public string OtpCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public bool IsUsed { get; set; }
    }
}