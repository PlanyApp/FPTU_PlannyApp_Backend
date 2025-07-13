using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ChallengeDetailDto
    {
        public int ChallengeId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? DifficultyLevel { get; set; }
        public int ProvinceId { get; set; }
        public int PackageId { get; set; }
        public int? PointsAwarded { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
    }

}
