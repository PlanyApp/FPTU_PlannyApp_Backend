using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ChallengeListItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string? PackageName { get; set; }
        public int? PointsAwarded { get; set; }
        public string? DifficultyLevel { get; set; }
        public bool IsActive { get; set; }
        public string? ProvinceName { get; set; }
    }

}
