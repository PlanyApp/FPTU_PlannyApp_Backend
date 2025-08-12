using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.Package
{
    public class UpdatePackageRequestDto
    {
        public string? Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public int? DurationDays { get; set; }

        public string? Type { get; set; }

        public string? Status { get; set; }
    }
}

