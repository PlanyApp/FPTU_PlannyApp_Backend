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
    public class PersonalChallengeApprovalService : IPersonalChallengeApprovalService
    {
        private readonly IUnitOfWork _uow;

        public PersonalChallengeApprovalService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //public async Task<List<PendingImageListDto>> GetPendingImagesAsync(int userPackageId)
        //{
        //    var list = await _uow.UserChallengeProgressRepository
        //        .FindIncludeAsync(
        //            uc => uc.UserPackageId == userPackageId
        //                  && uc.Status == "Submitted"
        //                  && uc.GroupId == null,
        //            uc => uc.Challenge,
        //            uc => uc.User,
        //            uc => uc.ProofImage
        //        );

        //    return list.Select(uc => new PendingImageListDto
        //    {
        //        UserChallengeProgressId = uc.UserChallengeProgressId,
        //        ChallengeName = uc.Challenge!.Name,
        //        UserFullName = uc.User.FullName,
        //        ImageUrl = uc.ProofImage!.ImageUrl
        //    }).ToList();
        //}


        public async Task<List<PendingImageListDto>> GetPendingImagesAsync()
        {
            // Lấy tất cả UserChallengeProgress cá nhân đang "Submitted"
            var list = await _uow.UserChallengeProgressRepository
                .FindIncludeAsync(
                    uc => uc.Status == "Submitted"
                          && uc.GroupId == null
                     && uc.ProofImageId != null,
                    uc => uc.Challenge,
                    uc => uc.User,
                    uc => uc.ProofImage
                );

            return list.Select(uc => new PendingImageListDto
            {
                UserChallengeProgressId = uc.UserChallengeProgressId,
                ChallengeName = uc.Challenge!.Name,
                UserFullName = uc.User!.FullName,
                ImageUrl = uc.ProofImage!.ImageUrl
            }).ToList();
        }
        public async Task<bool> ApproveImageAsync(int userChallengeProgressId, string? verificationNotes = null)
        {
            // 1. Lấy luôn kèm User và Challenge
            //var uc = await _uow.UserChallengeProgressRepository
            //    .FindIncludeAsync(
            //        x => x.UserChallengeProgressId == userChallengeProgressId,
            //        x => x.User,
            //        x => x.Challenge
            //    )
            //    .ContinueWith(t => t.Result.SingleOrDefault());

            //if (uc == null || uc.GroupId != null)
            //    return false;
            var list = await _uow.UserChallengeProgressRepository.FindIncludeAsync(
                       uc => uc.UserChallengeProgressId == userChallengeProgressId,
                       uc => uc.User,
                       uc => uc.Challenge
                   );
            var uc = list.SingleOrDefault();
            if (uc == null || uc.GroupId != null)
                return false;


            // 2. Cập nhật status, ghi chú
            uc.Status = "Completed";
            uc.VerificationNotes = verificationNotes;
            uc.CompletedAt = DateTime.UtcNow;

            // 3. Tính và gán PointsEarned, cộng vào user
            uc.PointsEarned = uc.Challenge!.PointsAwarded;           
          
            uc.User!.Points = (uc.User.Points ?? 0) + uc.PointsEarned;

            // 4. Update repository
            await _uow.UserChallengeProgressRepository.UpdateAsync(uc);
            await _uow.UserRepo2.UpdateAsync(uc.User);  // đảm bảo update luôn bảng User

            // 5. Save
            await _uow.SaveAsync();
            return true;
        }
        public async Task<bool> RejectImageAsync(int userChallengeProgressId)
        {
            var uc = await _uow.UserChallengeProgressRepository.GetByIdAsync(userChallengeProgressId);
            if (uc == null || uc.GroupId != null) return false;

            uc.Status = "Rejected";
            uc.CompletedAt = DateTime.UtcNow;
            // không gán VerificationNotes

            _uow.UserChallengeProgressRepository.Update(uc);
            await _uow.SaveAsync();
            return true;
        }

    }
}
