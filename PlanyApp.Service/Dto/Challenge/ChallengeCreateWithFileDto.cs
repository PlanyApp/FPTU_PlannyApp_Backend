using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ChallengeCreateWithFileDto
    {
        public IFormFile File { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? DifficultyLevel { get; set; }
        public int ProvinceId { get; set; }
        public int PackageId { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? PointsAwarded { get; set; }
    }

}
