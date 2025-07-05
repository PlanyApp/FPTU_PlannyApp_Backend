using PlanyApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ResponseGetListChallenge
    {
        //imge url add later
        public int ChallengeId;

        public string Name;

        public string? Description;

        public int? PackageId;

        public bool IsActive;
        public string imageUrl;


    }
}
