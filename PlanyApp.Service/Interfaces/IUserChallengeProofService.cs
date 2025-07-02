using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.ImageS3;
using PlanyApp.Service.Dto.UserChallengeProgress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IUserChallengeProofService
    {
        Task<ServiceResponseDto<ImageS3Dto>> UploadProofAsync(UploadProofImageRequestDto request);
    }

}
