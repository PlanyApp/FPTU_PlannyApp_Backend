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

            await _unitOfWork.GroupRepository.AddAsync(group);
            await _unitOfWork.SaveAsync();

            var member = new GroupMember
            {
                GroupId = group.GroupId,
                UserId = request.UserId,
                JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.GroupMemberRepository.AddAsync(member);
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
                JoinedAt = DateTime.UtcNow
            };

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
            //using var qrGen = new QRCodeGenerator();
            //// Giảm ECC level
            //using var qrData = qrGen.CreateQrCode(link, QRCodeGenerator.ECCLevel.L);
            //var svgQr = new SvgQRCode(qrData);
            //var svgText = svgQr.GetGraphic(2);

            //// Convert SVG string thành base64
            //var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(svgText));
            //return $"data:image/svg+xml;base64,{base64}";
            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(link, QRCodeGenerator.ECCLevel.L);
            var pngQr = new PngByteQRCode(qrData);
            var pngBytes = pngQr.GetGraphic(20); // scale pixel (điều chỉnh nếu cần)
            var base64 = Convert.ToBase64String(pngBytes);
            return $"data:image/png;base64,{base64}";

        }
    }
}
