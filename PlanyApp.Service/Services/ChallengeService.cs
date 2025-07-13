using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Challenge;
using PlanyApp.Service.Dto.ImageS3;
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
        private readonly IImageService _imageService;
        private const int MaxPageSize = 100;

        public ChallengeService(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
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
                imageUrl = c.ImageS3.ImageUrl ?? string.Empty // Assuming ImageUrl is a property in Challenge entity

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

        //========================================================================
        // ADMIN CHALLENGE 
        public async Task<ChallengeCreateResultDto> CreateChallengeAsync(ChallengeCreateWithFileDto dto)
        {
            // Step 1: Tạo challenge chưa có ảnh
            var challenge = new Challenge
            {
                Name = dto.Name,
                Description = dto.Description,
                DifficultyLevel = dto.DifficultyLevel,
                ProvinceId = dto.ProvinceId,
                PackageId = dto.PackageId,
                CreatedByUserId = dto.CreatedByUserId,
                PointsAwarded = dto.PointsAwarded ?? 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.ChallengeRepository.AddAsync(challenge);
            await _unitOfWork.SaveAsync(); // lúc này ChallengeId đã có

            // Step 2: Upload ảnh lên S3, gắn đúng ReferenceId
            var uploadResult = await _imageService.UploadImageAsync(new UploadImageRequestDto
            {
                File = dto.File,
                ReferenceType = "challenge",
                ReferenceId = challenge.ChallengeId
            });

            if (!uploadResult.IsSuccess || uploadResult.Data == null)
                throw new Exception("Image upload failed");

            var imageS3 = uploadResult.Data;

            // Step 3: Gán ImageS3Id vào challenge rồi cập nhật lại
            challenge.ImageS3Id = imageS3.ImageS3Id;
            _unitOfWork.ChallengeRepository.Update(challenge);
            await _unitOfWork.SaveAsync();

            return new ChallengeCreateResultDto
            {
                ChallengeId = challenge.ChallengeId,
                ImageUrl = imageS3.ImageUrl
            };
        }

        public async Task<ChallengeDetailDto?> GetChallengeByIdAsync(int id)
        {
            var challenge = await _unitOfWork.ChallengeRepository
                .FindIncludeAsync(c => c.ChallengeId == id, c => c.ImageS3);

            var entity = challenge.FirstOrDefault();
            if (entity == null) return null;

            return new ChallengeDetailDto
            {
                ChallengeId = entity.ChallengeId,
                Name = entity.Name,
                Description = entity.Description,
                DifficultyLevel = entity.DifficultyLevel,
                ProvinceId = entity.ProvinceId ?? 0,
                PackageId = entity.PackageId ?? 0,
                PointsAwarded = entity.PointsAwarded,
                IsActive = entity.IsActive,
                ImageUrl = entity.ImageS3?.ImageUrl
            };
        }
        public async Task<bool> PatchChallengeAsync(int id, ChallengePatchDto dto)
        {
            var challenge = await _unitOfWork.ChallengeRepository.GetByIdAsync(id);
            if (challenge == null) return false;

            if (dto.Name != null)
                challenge.Name = dto.Name;

            if (dto.Description != null)
                challenge.Description = dto.Description;

            if (dto.DifficultyLevel != null)
                challenge.DifficultyLevel = dto.DifficultyLevel;

            if (dto.ProvinceId.HasValue)
                challenge.ProvinceId = dto.ProvinceId.Value;

            if (dto.PackageId.HasValue)
                challenge.PackageId = dto.PackageId.Value;

            if (dto.PointsAwarded.HasValue)
                challenge.PointsAwarded = dto.PointsAwarded.Value;

            if (dto.File != null)
            {
                var uploadResult = await _imageService.UploadImageAsync(new UploadImageRequestDto
                {
                    File = dto.File,
                    ReferenceType = "challenge",
                    ReferenceId = challenge.ChallengeId
                });

                if (!uploadResult.IsSuccess || uploadResult.Data == null)
                    throw new Exception("Image upload failed");

                challenge.ImageS3Id = uploadResult.Data.ImageS3Id;
            }

            challenge.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.ChallengeRepository.Update(challenge);
            await _unitOfWork.SaveAsync();

            return true;
        }
        public async Task<bool?> ToggleChallengeStatusAsync(int challengeId)
        {
            var challenge = await _unitOfWork.ChallengeRepository.GetByIdAsync(challengeId);
            if (challenge == null) return null;

            challenge.IsActive = !challenge.IsActive;
            challenge.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.ChallengeRepository.Update(challenge);
            await _unitOfWork.SaveAsync();

            return challenge.IsActive;
        }
        //=================================================================
        // GET FILTERED CHALLENGES
        public async Task<(List<ChallengeListItemDto> Items, int TotalCount)> GetFilteredChallengesAsync(ChallengeListFilterDto filter)
        {
            if (filter.PageSize > 100)
                filter.PageSize = 100;

            if (filter.PageIndex < 1)
                filter.PageIndex = 1;

            var query = _unitOfWork.ChallengeRepository
                        .QueryInclude(c => c.Package, c => c.Province);
            

            if (filter.IsActive.HasValue)
                query = query.Where(c => c.IsActive == filter.IsActive.Value);

            if (filter.ProvinceId.HasValue)
                query = query.Where(c => c.ProvinceId == filter.ProvinceId.Value);

            if (filter.PackageId.HasValue)
                query = query.Where(c => c.PackageId == filter.PackageId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(c => c.Name.Contains(filter.Keyword));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.UpdatedAt)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(c => new ChallengeListItemDto
                {
                    Name = c.Name,
                    PackageName = c.Package != null ? c.Package.Name : null,
                    PointsAwarded = c.PointsAwarded,
                    DifficultyLevel = c.DifficultyLevel,
                    ProvinceName = c.Province != null ? c.Province.Name : null,
                    IsActive = c.IsActive
                })
                .ToListAsync();

            return (items, totalCount);
        }





    }
}
