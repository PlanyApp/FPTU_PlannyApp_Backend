using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.UserPackage
{
    public class ResponseListUserPackage
    {
        public int UserPackageId { get; set; }
        public int? PackageId { get; set; }
        public string PackageName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public int? GroupId { get; set; }
    }

}
