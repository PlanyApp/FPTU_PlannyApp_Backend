using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ChallengeGalleryWithInfoDto
    {
        public string ChallengeTitle { get; set; } = default!;
        public string? ChallengeImageUrl { get; set; }

        public string? CurrentUserStatus { get; set; }

        public List<string> Images { get; set; } = new();
    }

}
