namespace PlanyApp.Service.Dto.ImageS3
{
    public class ImageDto
    {
        public int ImageId { get; set; }
        public string ReferenceType { get; set; } = null!;
        public int ReferenceId { get; set; }
        public byte[] ImageData { get; set; } = null!;
        public string? ContentType { get; set; }
        public int? FileSizeKb { get; set; }
        public bool? IsPrimary { get; set; }
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 