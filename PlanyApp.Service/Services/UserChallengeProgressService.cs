using Microsoft.AspNetCore.Http;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Dto.UserChallengeProgress;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class UserChallengeProgressService : IUserChallengeProgressService
    {

        private readonly IUnitOfWork _unitOfWork;
        public UserChallengeProgressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ChallengeProgressDto>> GetChallengeProgressOfGroupAsync(int groupId, int challengeId)
        {
            var progresses = await _unitOfWork.UserChallengeProgressRepository
                .FindIncludeAsync(
                    p => p.GroupId == groupId && p.ChallengeId == challengeId,
                    p => p.User,           // Lấy thông tin user 
                    p => p.ProofImage      // Lấy ảnh minh chứng luôn nếu có
                );

            var result = progresses.Select(p => new ChallengeProgressDto
            {
                UserId = p.UserId,
                FullName = p.User?.FullName ?? "",
                Avatar = p.User?.Avatar ?? "",
                Status = p.Status,
                ProofImageUrl = p.ProofImage?.ImageUrl,
                VerificationNotes = p.VerificationNotes,
                PointsEarned = p.PointsEarned
            }).ToList();

            return result;
        }
        public async Task<int> CreateProgressAsync(int challengeId, int userPackageId, int userId)
        {
            // 1. Kiểm tra thử thách có tồn tại không
            var challenge = await _unitOfWork.ChallengeRepository.GetByIdAsync(challengeId);
            if (challenge == null)
                throw new ArgumentException("Challenge không tồn tại.");

            // 2. Lấy thông tin gói mà user truyền vào
            var userPackage = await _unitOfWork.UserPackageRepository.GetByIdAsync(userPackageId);

            // 2.1 Kiểm tra gói có tồn tại và có thuộc về user không
            if (userPackage == null || userPackage.UserId != userId)
                throw new InvalidOperationException("Gói không tồn tại hoặc không thuộc về bạn.");

            // 3. Kiểm tra gói này có đúng loại của thử thách không (PackageId phải khớp)
            if (userPackage.PackageId != challenge.PackageId)
                throw new InvalidOperationException("Gói này không áp dụng cho thử thách này.");

            // 4. Kiểm tra gói còn hiệu lực về mặt thời gian
            if (userPackage.StartDate > DateTime.UtcNow ||  // gói chưa bắt đầu
                (userPackage.EndDate != null && userPackage.EndDate < DateTime.UtcNow)) // hoặc đã hết hạn
            {
                throw new InvalidOperationException("Gói chưa bắt đầu hoặc đã hết hạn.");
            }

            // 5. Kiểm tra xem user đã tạo progress cho challenge này bằng chính gói này chưa
            var exists = await _unitOfWork.UserChallengeProgressRepository.FirstOrDefaultAsync(p =>
                p.UserId == userId &&
                p.ChallengeId == challengeId &&
                p.UserPackageId == userPackageId &&
                (p.Status == "Started" || p.Status == "Submitted")); // chỉ cấm khi chưa hoàn thành

            if (exists !=null)
                throw new InvalidOperationException("Bạn đã tham gia thử thách này bằng gói này rồi.");
            
            // 6. Tạo mới dòng UserChallengeProgress
            var progress = new UserChallengeProgress
            {
                UserId = userId,
                ChallengeId = challengeId,
                UserPackageId = userPackageId, // gắn đúng lượt mua
                Status = "Started",
                StartedAt = DateTime.UtcNow, 
                GroupId = userPackage.GroupId
            };

            // 7. Lưu vào database
            await _unitOfWork.UserChallengeProgressRepository.AddAsync(progress);
            await _unitOfWork.SaveAsync();

            // 8. Trả về ID của dòng vừa tạo
            return progress.UserChallengeProgressId;
        }

    }
}
