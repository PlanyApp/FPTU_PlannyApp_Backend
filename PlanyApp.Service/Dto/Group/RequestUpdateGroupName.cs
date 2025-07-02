using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Group
{
    public class RequestUpdateGroupName
    {
        public int GroupId { get; set; }
        public string NewName { get; set; } = string.Empty;
    }

}
