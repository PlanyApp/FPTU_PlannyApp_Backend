using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.Package
{
    public class CreatePackageRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int? DurationDays { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // e.g., "group" or other

        [Required]
        public string Status { get; set; } = "Active";
    }
}

