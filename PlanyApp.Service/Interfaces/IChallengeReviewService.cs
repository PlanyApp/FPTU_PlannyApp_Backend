using PlanyApp.Service.Dto.ChallengeReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IChallengeReviewService
    {
        Task<List<PendingGroupChallengeDto>> GetPendingGroupChallengesAsync();
        Task<List<ImageDetailDto>> FetchImagesForReviewAsync(GetImageDetailsRequestDto request);
        Task ApproveSelectionAsync(ApproveImageSelectionRequestDto request);

    }

}
