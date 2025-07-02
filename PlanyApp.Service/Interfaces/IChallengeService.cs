using PlanyApp.Service.Dto.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IChallengeService
    {
        Task<List<ResponseGetListChallenge>> GetChallengesByPackageIdAsync(int packageId, int provinceId);

        Task<string?> GetChallengeDescriptionAsync(int challengeId);

       
        Task<ProgressChallengeImageListDto> GetGroupChallengeImagesAsync(int challengeId, int groupId, int currentUserId, int userPackageId);
        Task<PersonalChallengeProgressDto> GetPersonalChallengeProgressAsync(int challengeId, int currentUserId, int userPackageId);


    }
}
