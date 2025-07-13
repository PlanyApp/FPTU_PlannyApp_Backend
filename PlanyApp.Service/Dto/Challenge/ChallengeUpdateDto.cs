using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ChallengePatchDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? DifficultyLevel { get; set; }
        public int? ProvinceId { get; set; }
        public int? PackageId { get; set; }
        public int? PointsAwarded { get; set; }

        // Ảnh mới (tuỳ chọn)
        public IFormFile? File { get; set; }


    }
}
