using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.ChallengeReview
{
    public class PendingGroupChallengeDto
    {
        public int ChallengeId { get; set; }
        public string ChallengeName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int TotalPendingImages { get; set; }
        public List<int> UserChallengeProgressIds { get; set; } = new();
    }

}
