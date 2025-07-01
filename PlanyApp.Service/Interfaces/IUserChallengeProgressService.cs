using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Dto.UserChallengeProgress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IUserChallengeProgressService
    {
        public Task<List<ChallengeProgressDto>> GetChallengeProgressOfGroupAsync(int groupId, int challengeId);


        Task<int> CreateProgressAsync(int challengeId, int userId, int userPackageId);
    }


}
