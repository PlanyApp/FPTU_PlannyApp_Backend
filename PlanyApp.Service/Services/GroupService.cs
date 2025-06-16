using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Group;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private const int DefaultGroupPackageId = 4;
        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public Task<InviteLinkDto> GenerateInviteLinkAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public bool ValidateInviteLink(GroupInviteRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> JoinGroupAsync(GroupInviteRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
