using PlanyApp.Service.Dto.UserChallengeProgress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IUserChallengeProgress
    {
        public Task<List<ChallengeProgressDto>> GetChallengeProgressOfGroupAsync(int groupId, int challengeId);

    }
}
