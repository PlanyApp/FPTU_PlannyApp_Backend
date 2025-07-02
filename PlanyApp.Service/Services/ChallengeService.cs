using Microsoft.EntityFrameworkCore.Scaffolding;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Challenge;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class ChallengeService : IChallengeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChallengeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<List<ResponseGetListChallenge>> GetChallengesByPackageIdAsync(int packageId, int provinceId)
        {
            var challenges = await _unitOfWork.ChallengeRepository.FindAsync(c => c.PackageId == packageId && c.IsActive==true && c.ProvinceId== provinceId  );
            //&& c.Province == province
            return challenges.Select(c => new ResponseGetListChallenge
            {
                ChallengeId = c.ChallengeId,
                Name = c.Name,
                Description = c.Description,
                PackageId = c.PackageId,
                //IsActive = c.IsActive
                // image url add later
               
            }).ToList();
        }
        public async Task<string?> GetChallengeDescriptionAsync(int challengeId)
        {
            var challenge = await _unitOfWork.ChallengeRepository.GetByIdAsync(challengeId);
            if (challenge == null) return null;

            return challenge.Description ?? string.Empty;
        }
        public async Task<ProgressChallengeImageListDto> GetGroupChallengeImagesAsync(
      int challengeId,
      int groupId,
      int currentUserId,
      int userPackageId)
        {
            var progressList = await _unitOfWork.UserChallengeProgressRepository
                .FindIncludeAsync(
                    ucp => ucp.ChallengeId == challengeId
                        && ucp.GroupId == groupId
                        && ucp.UserPackageId == userPackageId,
                    ucp => ucp.ProofImage
                );

            var imageUrls = progressList
                .Where(x => x.ProofImage != null)
                .Select(x => x.ProofImage!.ImageUrl)
                .ToList();

            var currentUserStatus = progressList
                .FirstOrDefault(x => x.UserId == currentUserId)?.Status ?? "NotStarted";

            return new ProgressChallengeImageListDto
            {
                CurrentUserStatus = currentUserStatus,
                Images = imageUrls
            };
        }

        public async Task<PersonalChallengeProgressDto> GetPersonalChallengeProgressAsync(
      int challengeId,
      int currentUserId,
      int userPackageId)
        {
            var progressList = await _unitOfWork.UserChallengeProgressRepository
                .FindIncludeAsync(
                    x => x.ChallengeId == challengeId
                        && x.UserId == currentUserId
                        && x.UserPackageId == userPackageId,
                    x => x.ProofImage
                );

            var progress = progressList.FirstOrDefault();

            var status = progress?.Status ?? "NotStarted";
            var imageUrl = progress?.ProofImage?.ImageUrl;

            return new PersonalChallengeProgressDto
            {
                Status = status,
                ProofImageUrl = imageUrl
            };
        }




    }
}
