using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ProgressChallengeImageListDto
    {
        public string CurrentUserStatus { get; set; } = "NotStarted";
        public List<string> Images { get; set; } = new();
    }


}
