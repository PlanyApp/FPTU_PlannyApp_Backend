using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PlanyApp.Service.Services
{
    public class DisabledImageService : IImageService
    {
        public Task<ServiceResponseDto<ImageS3Dto>> UploadImageAsync(UploadImageRequestDto request)
        {
            var response = new ServiceResponseDto<ImageS3Dto>
            {
                IsSuccess = false,
                Message = "Image upload functionality is not configured on the server. Please configure S3 settings.",
                Errors = new System.Collections.Generic.List<string> { "S3_NOT_CONFIGURED" }
            };
            return Task.FromResult(response);
        }

        public Task<ServiceResponseDto<List<ImageS3Dto>>> GetImagesByReferenceAsync(string referenceType, int referenceId)
        {
            var response = new ServiceResponseDto<List<ImageS3Dto>>
            {
                IsSuccess = false,
                Message = "Image service functionality is not configured on the server. Please configure S3 settings.",
                Errors = new List<string> { "S3_NOT_CONFIGURED" }
            };
            return Task.FromResult(response);
        }

        public Task<ServiceResponseDto<object>> GetImagesByReferenceAsync(string referenceType, int referenceId, bool isS3)
        {
            var response = new ServiceResponseDto<object>
            {
                IsSuccess = false,
                Message = isS3 
                    ? "S3 image service functionality is not configured on the server. Please configure S3 settings."
                    : "Regular image service functionality is available, but database access is not configured properly.",
                Errors = new List<string> { isS3 ? "S3_NOT_CONFIGURED" : "DATABASE_ACCESS_ERROR" }
            };
            return Task.FromResult(response);
        }
    }
} 