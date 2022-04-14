using System.ComponentModel.DataAnnotations;

namespace Hire360WebAPI.Models
{
    public class ForgotPassword
    {
        [Required]
        public string? Email { get; set; }

        public string? Password { get; set; }
        public string? Otp { get; set; }
    }
}