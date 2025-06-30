using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PlanyApp.Service.Dto.ImageS3
{
    public class UploadImageRequestDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
        [Required]
        public string ReferenceType { get; set; } = null!;
        [Required]
        public int ReferenceId { get; set; }
        public string? Caption { get; set; }
        public bool? IsPrimary { get; set; }
    }
} 