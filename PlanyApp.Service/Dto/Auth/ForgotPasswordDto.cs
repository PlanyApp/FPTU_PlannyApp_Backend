using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
} 