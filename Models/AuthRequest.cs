using System.ComponentModel.DataAnnotations;

namespace Hire360WebAPI.Models
{
    public class AuthRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}