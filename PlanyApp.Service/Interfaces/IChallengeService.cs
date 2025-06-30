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

        /// <summary>
        /// Lấy thông tin challenge và gallery ảnh của group kèm trạng thái user hiện tại
        /// </summary>
        //Task<ChallengeGalleryWithInfoDto?> GetChallengeGalleryWithInfoAsync(
        //    int challengeId, int groupId, int currentUserId);
    }
}
