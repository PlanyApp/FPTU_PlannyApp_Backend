using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.Group
{
    public class GroupInviteRequestDto
    {
        public int GroupId { get; set; }
        public long Ts { get; set; }
        public string Sig { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
