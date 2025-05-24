using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }
    }
} 