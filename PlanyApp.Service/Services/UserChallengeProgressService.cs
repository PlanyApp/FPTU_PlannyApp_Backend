using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.UserChallengeProgress;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class UserChallengeProgressService: IUserChallengeProgress
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserChallengeProgressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ChallengeProgressDto>> GetChallengeProgressOfGroupAsync(int groupId, int challengeId)
        {
            var progresses = await _unitOfWork.UserChallengeProgressRepository
                .FindIncludeAsync(
                    p => p.GroupId == groupId && p.ChallengeId == challengeId,
                    p => p.User,           // Lấy thông tin user 
                    p => p.ProofImage      // Lấy ảnh minh chứng luôn nếu có
                );

            var result = progresses.Select(p => new ChallengeProgressDto
            {
                UserId = p.UserId,
                FullName = p.User?.FullName ?? "",
                Avatar = p.User?.Avatar ?? "",
                Status = p.Status,
               // ProofImageUrl = p.ProofImage?.ImageData,
                VerificationNotes = p.VerificationNotes,
                PointsEarned = p.PointsEarned
            }).ToList();

            return result;
        }

    }
}
