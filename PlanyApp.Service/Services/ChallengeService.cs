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
        //public async Task<ChallengeGalleryWithInfoDto?> GetChallengeGalleryWithInfoAsync(int challengeId, int groupId, int currentUserId)
        //{
        //    // 1. Lấy challenge
        //    var challenge = await _unitOfWork.ChallengeRepository.GetByIdAsync(challengeId);
        //    if (challenge == null) return null;

        //    // 2. Lấy danh sách progress
        //    var progresses = await _unitOfWork.UserChallengeProgressRepository
        //        .FindIncludeAsync(
        //            p => p.ChallengeId == challengeId && p.GroupId == groupId,
        //            p => p.ProofImage
        //        );

        //    // 3. Lấy trạng thái của người đang đăng nhập
        //    var currentUserStatus = progresses
        //        .FirstOrDefault(p => p.UserId == currentUserId)?.Status;

        //    // 4. Lấy danh sách hình ảnh
        //    var imageUrls = progresses
        //        .Where(p => p.ProofImage != null)
        //        .Select(p => p.ProofImage!.ImageData!)
        //        .ToList();

        //    return new ChallengeGalleryWithInfoDto
        //    {
        //        ChallengeTitle = challenge.Name,
        //        ChallengeImageUrl = challenge.Image,
        //        CurrentUserStatus = currentUserStatus,
        //        Images = imageUrls
        //    };
        //}


    }
}
