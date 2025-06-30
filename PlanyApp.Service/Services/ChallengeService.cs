using Microsoft.EntityFrameworkCore.Scaffolding;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Challenge;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class ChallengeService : IChallengeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChallengeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<List<ResponseGetListChallenge>> GetChallengesByPackageIdAsync(int packageId, int provinceId)
        {
            var challenges = await _unitOfWork.ChallengeRepository.FindAsync(c => c.PackageId == packageId && c.IsActive==true && c.ProvinceId== provinceId  );
            //&& c.Province == province
            return challenges.Select(c => new ResponseGetListChallenge
            {
                ChallengeId = c.ChallengeId,
                Name = c.Name,
                Description = c.Description,
                PackageId = c.PackageId,
                //IsActive = c.IsActive
                // image url add later
               
            }).ToList();
        }
    }
}
