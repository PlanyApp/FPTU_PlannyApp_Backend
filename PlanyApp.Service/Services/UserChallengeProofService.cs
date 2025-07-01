using Microsoft.AspNetCore.Http;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Dto.UserChallengeProgress;
using PlanyApp.Service.Interfaces;
using System.Security.Claims;

namespace PlanyApp.Service.Services
{
    public class UserChallengeProofService : IUserChallengeProofService
    {
        private readonly IImageService _imageService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserChallengeProofService(
            IImageService imageService,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _imageService = imageService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponseDto<ImageS3Dto>> UploadProofAsync(UploadProofImageRequestDto request)
        {
            // 1. Lấy UserId từ token
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                               ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub");

            if (userIdClaim == null)
            {
                return new ServiceResponseDto<ImageS3Dto>(
                    success: false,
                    message: "User is not authenticated."
                );
            }

            int userId = int.Parse(userIdClaim.Value);
            // 2. Kiểm tra request hợp lệ
            var progress = await _unitOfWork.UserChallengeProgressRepository.GetByIdAsync(request.ReferenceId);
            if (progress.Status == "Completed" || progress.Status == "Rejected")
            {
                throw new InvalidOperationException("Thử thách này đã được chấm điểm hoặc bị từ chối, không thể sửa ảnh.");
            }
            if (progress == null || progress.UserId != userId)
            {
                return new ServiceResponseDto<ImageS3Dto>(
                    success: false,
                    message: "Progress not found or does not belong to current user."
                );
            }

            // 3. Chuẩn bị DTO gốc cho ImageService
            var imageUploadRequest = new UploadImageRequestDto
            {
                File = request.File,
                ReferenceId = request.ReferenceId,
                Caption = request.Caption,
                IsPrimary = request.IsPrimary,
                ReferenceType = "UserChallengeProgress" 
            };

            var result = await _imageService.UploadImageAsync(imageUploadRequest);
            if (!result.IsSuccess || result.Data == null)
            {
                return new ServiceResponseDto<ImageS3Dto>(
                    success: false,
                    message: result.Message,
                    data: null,
                    errors: result.Errors
                );
            }

           

            // 4. Tìm image vừa upload
            var image = await _unitOfWork.ImageS3Repository.FirstOrDefaultAsync(
                i => i.ImageUrl == result.Data.ImageUrl);

            if (image == null)
            {
                return new ServiceResponseDto<ImageS3Dto>(
                    success: false,
                    message: "Image record not found after upload."
                );
            }
         
          
         

            // 5. Gán ảnh
            progress.ProofImageId = image.ImageS3id;
            progress.Status = "Submitted";
            await _unitOfWork.UserChallengeProgressRepository.UpdateAsync(progress);
            await _unitOfWork.SaveAsync();

            // 6. Trả kết quả
            return new ServiceResponseDto<ImageS3Dto>(
                success: true,
                message: "Image uploaded and proof updated.",
                data: result.Data
            );
        }

    }
}
