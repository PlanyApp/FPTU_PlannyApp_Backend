using Microsoft.Extensions.Configuration;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Interfaces;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace PlanyApp.Service.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int DefaultGroupPackageId = 4;
        private readonly string _secretKey;
        private readonly string _appDomain;

        public GroupService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _secretKey = config["InviteConfig:SecretKey"] ?? throw new Exception("SecretKey missing");
            _appDomain = config["InviteConfig:AppDomain"] ?? throw new Exception("AppDomain missing");
        }
        public async Task<Group> CreateGroupAsync(CreateGroupRequest request)
        {
            var now = DateTime.UtcNow;

            var group = new Group
            {
                Name = request.GroupName,
                OwnerId = request.UserId,
                CreatedAt = now,
                UpdatedAt = now,
                GroupStart = now,
                GroupEnd = now.AddMonths(1),
                Description = $"Group created by user {request.UserId}",
                IsPrivate = false, // default or based on logic
                TotalMember = 1, // owner is first member
                GroupPackage = DefaultGroupPackageId
            };

            _unitOfWork.GroupRepository.Add(group);
            await _unitOfWork.SaveAsync();

            var member = new GroupMember
            {
                GroupId = group.GroupId,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow
            };

            _unitOfWork.GroupMemberRepository.Add(member);
            await _unitOfWork.SaveAsync();

            return group;
        }

        public async Task<InviteLinkDto> GenerateInviteLinkAsync(int groupId)
        {
            var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var sig = CreateSignature(groupId, ts);
            var link = $"{_appDomain}/invite/group/{groupId}?ts={ts}&sig={sig}";

            //var qrBytes = GenerateQrCodeBytes(link);
            //var qrBase64 = Convert.ToBase64String(qrBytes);
            var qrUrl = GenerateQrCodeSvgBase64(link);

            return await Task.FromResult(new InviteLinkDto
            {
                InviteLink = link,
                QrUrl = qrUrl
            });
        }

        public bool ValidateInviteLink(GroupInviteRequestDto request)
        {
            var nowTs = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (nowTs - request.Ts > 60)
                return false;  // Link hết hạn

            var expectedSig = CreateSignature(request.GroupId, request.Ts);
            return expectedSig == request.Sig;
        }

        public async Task<bool> JoinGroupAsync(GroupInviteRequestDto request)
        {
            if (!ValidateInviteLink(request))
                return false;

            var exists = await _unitOfWork.GroupMemberRepository
                .FirstOrDefaultAsync(gm => gm.GroupId == request.GroupId && gm.UserId == request.UserId);
            if (exists != null)
                return false;

            var member = new GroupMember
            {
                GroupId = request.GroupId,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow,
                RoleInGroup = "Member",
                CashContributed = 0m
            };
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(request.GroupId);
            var newUserPackage = new UserPackage
            {
                UserId = request.UserId,
                PackageId = group.GroupPackage.Value,
                StartDate = DateTime.UtcNow,
                IsActive = true,
                
                EndDate = DateTime.UtcNow.AddMonths(1),
                GroupId = request.GroupId

            };
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(request.GroupId);
            var newUserPackage = new UserPackage
            {
                UserId = request.UserId,
                PackageId = group.GroupPackage.Value,
                StartDate = DateTime.UtcNow,
                IsActive = true,
                
            };

            await _unitOfWork.UserPackageRepository.AddAsync(newUserPackage);


            await _unitOfWork.UserPackageRepository.AddAsync(newUserPackage);
            await _unitOfWork.GroupMemberRepository.AddAsync(member);
            await _unitOfWork.SaveAsync();

            return true;
        }
        private string CreateSignature(int groupId, long ts)
        {
            var data = $"{groupId}:{ts}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        private string GenerateQrCodeSvgBase64(string link)
        {
            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(link, QRCodeGenerator.ECCLevel.L);
            var pngQr = new PngByteQRCode(qrData);
            var pngBytes = pngQr.GetGraphic(20); // scale pixel (điều chỉnh nếu cần)
            var base64 = Convert.ToBase64String(pngBytes);
            return $"data:image/png;base64,{base64}";
        }

        //--------------------------------------------------------
        public async Task<GroupDetailDto?> GetGroupDetailsAsync(int groupId)
        {
            var group = await _unitOfWork.GroupRepository
                .FindIncludeAsync(
                    g => g.GroupId == groupId,
                    g => g.GroupPackageNavigation // join với bảng Packages
                );

            var groupEntity = group.FirstOrDefault();
            if (groupEntity == null) return null;

            var userPackage = await _unitOfWork.UserPackageRepository
                .FirstOrDefaultAsync(up => up.PackageId == groupEntity.GroupPackage);

            return new GroupDetailDto
            {
                GroupName = groupEntity.Name,
                PackageName = groupEntity.GroupPackageNavigation?.Name ?? "(No package)",
                ExpiryDate = userPackage?.EndDate
            };
        }


        //---------------------------------------------------------
        public async Task<bool> UpdateGroupNameAsync(RequestUpdateGroupName request)
        {
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(request.GroupId);
            if (group == null)
                throw new Exception("Group not found");

            if (string.IsNullOrWhiteSpace(request.NewName))
                throw new Exception("Tên nhóm không được để trống");

            group.Name = request.NewName;
            group.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GroupRepository.Update(group);
            await _unitOfWork.SaveAsync();

            return true;
        }

    }
}
