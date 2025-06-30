using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.ImageS3;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PlanyApp.Service.Interfaces
{
    public interface IImageService
    {
        Task<ServiceResponseDto<ImageS3Dto>> UploadImageAsync(UploadImageRequestDto request);
        Task<ServiceResponseDto<List<ImageS3Dto>>> GetImagesByReferenceAsync(string referenceType, int referenceId);
        Task<ServiceResponseDto<object>> GetImagesByReferenceAsync(string referenceType, int referenceId, bool isS3);
    }
} 