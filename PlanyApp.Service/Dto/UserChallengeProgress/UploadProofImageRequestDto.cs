using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Dto.UserChallengeProgress
{
    public class UploadProofImageRequestDto
    {
        public IFormFile File { get; set; } = null!;
        public int ReferenceId { get; set; } // chính là UserChallengeProgressId
        public string? Caption { get; set; }
        public bool? IsPrimary { get; set; }
    }

}
