using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Group
{
    public class CreateGroupRequest
    {
        public int UserId { get; set; }
     
        public string GroupName { get; set; }
    }
}
