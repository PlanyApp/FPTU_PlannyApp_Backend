using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.Auth
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string? Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        [Phone]
        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? City { get; set; }

        public decimal? MonthlyIncome { get; set; }
    }
} 