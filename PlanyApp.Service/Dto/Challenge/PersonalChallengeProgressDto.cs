using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class PersonalChallengeProgressDto
    {
        public string Status { get; set; } = "NotStarted";
        public string? ProofImageUrl { get; set; }
    }

}
