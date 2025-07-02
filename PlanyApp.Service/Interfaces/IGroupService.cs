using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IGroupService
    {
        Task<Group> CreateGroupAsync(CreateGroupRequest request);
        Task<InviteLinkDto> GenerateInviteLinkAsync(int groupId);
        bool ValidateInviteLink(GroupInviteRequestDto request);
        Task<bool> JoinGroupAsync(GroupInviteRequestDto request);
        Task<GroupDetailDto?> GetGroupDetailsAsync(int groupId);
        Task<bool> UpdateGroupNameAsync(RequestUpdateGroupName request);

    }
}
