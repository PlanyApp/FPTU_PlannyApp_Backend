using Microsoft.EntityFrameworkCore;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.ChallengeReview;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class ChallengeReviewService : IChallengeReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChallengeReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PendingGroupChallengeDto>> GetPendingGroupChallengesAsync()
        {
            // 1. Lấy toàn bộ progress đã submit có ảnh và group
            var ucpList = await _unitOfWork.UserChallengeProgressRepository.GetAllIncludeAsync(
                x => x.User,
                x => x.Challenge,
                x => x.Group
            );

            // 2. Lấy tất cả userPackage để kiểm tra gói
            var userPackages = await _unitOfWork.UserPackageRepository.GetAllAsync();

            // 3. Dùng dictionary để gom theo (ChallengeId, GroupId)
            var map = new Dictionary<(int challengeId, int groupId), PendingGroupChallengeDto>();

            foreach (var ucp in ucpList)
            {
                // Lọc chỉ “Submitted” + có ảnh + là nhóm
                if (ucp.Status != "Submitted"
                    || ucp.ProofImageId == null
                    || ucp.GroupId == null)
                    continue;

                var group = ucp.Group!;
                var up = userPackages.FirstOrDefault(p => p.UserPackageId == ucp.UserPackageId);

                // Bỏ qua nếu gói user không trùng với group.PackageId
                if (up == null || up.PackageId != group.GroupPackage)
                    continue;

                var key = (ucp.ChallengeId, group.GroupId);

                if (!map.TryGetValue(key, out var dto))
                {
                    // Tạo entry mới
                    dto = new PendingGroupChallengeDto
                    {
                        ChallengeId = ucp.ChallengeId,
                        ChallengeName = ucp.Challenge!.Name,
                        GroupId = group.GroupId,
                        GroupName = group.Name,
                        TotalPendingImages = 0
                    };
                    map[key] = dto;
                }

                // Tăng count và thêm UCPId vào list
                dto.TotalPendingImages++;
                dto.UserChallengeProgressIds.Add(ucp.UserChallengeProgressId);
            }

            return map.Values.ToList();
        }

        //-----------------------
        public async Task<List<ImageDetailDto>> FetchImagesForReviewAsync(GetImageDetailsRequestDto request)
        {
            if (request.UserChallengeProgressIds == null || !request.UserChallengeProgressIds.Any())
                return new List<ImageDetailDto>();

            // 1. Lấy các progress qua repository, include navigation tới ProofImage
            var progresses = await _unitOfWork.UserChallengeProgressRepository
                .FindIncludeAsync(
                    ucp => request.UserChallengeProgressIds.Contains(ucp.UserChallengeProgressId),
                    ucp => ucp.ProofImage    // navigation vào ImageS3
                );

            // 2. Map về DTO
            var result = progresses
                .Select(ucp => new ImageDetailDto
                {
                    UserChallengeProgressId = ucp.UserChallengeProgressId,
                    ImageUrl = ucp.ProofImage!.ImageUrl
                })
                .ToList();

            return result;
        }
        //------------------------
        /// <summary>
        /// Mark the chosen progress as Completed; others as Rejected.
        /// </summary>
        public async Task ApproveSelectionAsync(ApproveImageSelectionRequestDto request)
        {
            // nothing to do if list empty
            if (request.UserChallengeProgressIds == null || !request.UserChallengeProgressIds.Any())
                return;

            // load all affected progress entries
            var progresses = await _unitOfWork.UserChallengeProgressRepository
                .FindAsync(ucp => request.UserChallengeProgressIds.Contains(ucp.UserChallengeProgressId));

            foreach (var ucp in progresses)
            {
                // approved => Completed, else => Rejected
                ucp.Status = ucp.UserChallengeProgressId == request.ApprovedProgressId
                    ? "Completed"
                    : "Rejected";
                // ghi nhận note cho record được duyệt
                if (ucp.UserChallengeProgressId == request.ApprovedProgressId)
                {
                    ucp.VerificationNotes = request.VerificationNotes;
                    ucp.PointsEarned = ucp.Challenge.PointsAwarded ;
                    // cập nhật điêểm cho người dùng
                    ucp.User!.Points = (ucp.User.Points ?? 0) + ucp.PointsEarned;
                }

                _unitOfWork.UserChallengeProgressRepository.Update(ucp);
            }

            // persist changes
            await _unitOfWork.SaveAsync();
        }


    }

}
