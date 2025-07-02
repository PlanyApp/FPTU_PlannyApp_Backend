using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Group
{
    public class GroupDetailDto
    {
        public string GroupName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
    }

}
