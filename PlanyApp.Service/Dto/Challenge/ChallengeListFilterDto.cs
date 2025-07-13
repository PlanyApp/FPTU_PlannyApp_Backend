using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Challenge
{
    public class ChallengeListFilterDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsActive { get; set; }
        public int? ProvinceId { get; set; }
        public int? PackageId { get; set; }
        public string? Keyword { get; set; }
    }

}
