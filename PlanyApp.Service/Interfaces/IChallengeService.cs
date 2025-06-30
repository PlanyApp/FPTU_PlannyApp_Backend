using PlanyApp.Service.Dto.Challenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IChallengeService
    {
        Task<List<ResponseGetListChallenge>> GetChallengesByPackageIdAsync(int packageId, int provinceId);

    }
}
