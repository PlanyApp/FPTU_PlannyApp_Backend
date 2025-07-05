using PlanyApp.Service.Dto.ChallengeReview;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IPersonalChallengeApprovalService
    {
       
        Task<List<PendingImageListDto>> GetPendingImagesAsync();
        Task<bool> ApproveImageAsync(int userChallengeProgressId, string? verificationNotes = null);
        Task<bool> RejectImageAsync(int userChallengeProgressId);
    }
}
