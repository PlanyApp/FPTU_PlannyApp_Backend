using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.Auth
{
    public class GoogleLoginRequestDto
    {
        [Required]
        public string IdToken { get; set; }
    }
} 