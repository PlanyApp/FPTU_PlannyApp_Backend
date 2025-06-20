using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.UserChallengeProgress
{
   
    public class ChallengeProgressDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        public string? ProofImageUrl { get; set; }
        public string? VerificationNotes { get; set; }
        public int? PointsEarned { get; set; }
    }

}
