using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace PlanyApp.Service.Services
{
    public class PlanAccessService : IPlanAccessService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanAccessService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CanViewPlanAsync(int userId, int planId)
        {
            // Owner can view
            var plan = await _unitOfWork.PlanRepository.GetByIdAsync(planId);
            if (plan == null) return false;
            if (plan.OwnerId == userId) return true;
            if (plan.IsPublic) return true;

            // Find the single group linked to this plan
            var groups = await _unitOfWork.GroupRepository.FindAsync(g => g.PlanId == planId);
            if (groups.Count == 0) return false;

            // Require the plan owner to have active group package for that group
            foreach (var group in groups)
            {
                if (await HasActiveGroupPackage(plan.OwnerId, group) && await IsGroupMemberWithActivePackage(userId, group))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CanEditPlanAsync(int userId, int planId)
        {
            // Owner can edit
            var plan = await _unitOfWork.PlanRepository.GetByIdAsync(planId);
            if (plan == null) return false;
            if (plan.OwnerId == userId) return true;

            // Find the single group linked to this plan
            var groups = await _unitOfWork.GroupRepository.FindAsync(g => g.PlanId == planId);
            if (groups.Count == 0) return false;

            // Require the plan owner to have active group package for that group
            foreach (var group in groups)
            {
                if (await HasActiveGroupPackage(plan.OwnerId, group) && await IsGroupMemberWithActivePackage(userId, group))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> LinkPlanToGroupAsync(int planId, int groupId, int userId)
        {
            var plan = await _unitOfWork.PlanRepository.GetByIdAsync(planId);
            if (plan == null) return false;
            var group = await _unitOfWork.GroupRepository.GetByIdAsync(groupId);
            if (group == null) return false;

            // Enforce single-group assignment: if plan already linked to another group, do not allow
            var existingLinks = await _unitOfWork.GroupRepository.FindAsync(g => g.PlanId == planId);
            if (existingLinks.Any(g => g.GroupId != groupId))
            {
                return false;
            }

            // Group must have a package
            if (!group.GroupPackage.HasValue) return false;

            // Acting user must be plan owner OR group owner
            var isOwner = plan.OwnerId == userId || group.OwnerId == userId;
            if (!isOwner) return false;

            // Require acting user has active group package for this group
            var actorHasPackage = await HasActiveGroupPackage(userId, group);
            if (!actorHasPackage) return false;

            // Require plan owner has active group package for this group
            var planOwnerHasPackage = await HasActiveGroupPackage(plan.OwnerId, group);
            if (!planOwnerHasPackage) return false;

            group.PlanId = planId;
            group.UpdatedAt = System.DateTime.UtcNow;
            _unitOfWork.GroupRepository.Update(group);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private async Task<bool> IsGroupMemberWithActivePackage(int userId, Group group)
        {
            var isMember = await _unitOfWork.GroupMemberRepository.ExistsAsync(gm => gm.GroupId == group.GroupId && gm.UserId == userId);
            if (!isMember) return false;

            return await HasActiveGroupPackage(userId, group);
        }

        private async Task<bool> HasActiveGroupPackage(int userId, Group group)
        {
            if (!group.GroupPackage.HasValue) return false;

            var now = DateTime.UtcNow;
            var hasActive = await _unitOfWork.UserPackageRepository.ExistsAsync(up =>
                up.UserId == userId &&
                up.GroupId == group.GroupId &&
                up.PackageId == group.GroupPackage.Value &&
                (up.IsActive ?? false) &&
                up.StartDate <= now &&
                up.EndDate >= now);

            return hasActive;
        }
    }
} 