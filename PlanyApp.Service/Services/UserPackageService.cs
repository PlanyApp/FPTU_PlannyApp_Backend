using Microsoft.EntityFrameworkCore;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.UserPackage;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class UserPackageService : IUserPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserPackageService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<List<ResponseListUserPackage>> GetPackageIdsByUserIdAsync(int userId)
        {
            var query = _unitOfWork.UserPackageRepository
           .Query()
           .Where(up => up.UserId == userId)
           .Include(up => up.Package)
           .Select(up => new ResponseListUserPackage
           {
               UserPackageId = up.UserPackageId,
               PackageId = up.PackageId,
               PackageName = up.Package.Name,
               StartDate = up.StartDate,
               EndDate = up.EndDate,
               Description = up.Package.Description,
               IsActive = up.IsActive, 
               GroupId = up.GroupId
           });

            return await query.ToListAsync();
        }

        public async Task<ResponseListUserPackage?> GetCurrentPackageByUserIdAsync(int userId)
        {
            var now = DateTime.UtcNow;
            var query = _unitOfWork.UserPackageRepository
                .Query()
                .Where(up => up.UserId == userId)
                .Include(up => up.Package)
                .Where(up => (up.IsActive == true) && up.StartDate <= now && up.EndDate >= now)
                .OrderByDescending(up => up.StartDate)
                .Select(up => new ResponseListUserPackage
                {
                    UserPackageId = up.UserPackageId,
                    PackageId = up.PackageId,
                    PackageName = up.Package.Name,
                    StartDate = up.StartDate,
                    EndDate = up.EndDate,
                    Description = up.Package.Description,
                    IsActive = up.IsActive,
                    GroupId = up.GroupId
                });

            return await query.FirstOrDefaultAsync();
        }
    }
}
