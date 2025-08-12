using PlanyApp.Service.Dto.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IPackageService
    {
        Task<List<PackageDto>> GetAllPackagesAsync();
        Task<PackageDto?> GetByIdAsync(int packageId);
        Task<PackageDto> CreateAsync(Dto.Package.CreatePackageRequestDto request);
        Task<PackageDto?> UpdateAsync(int packageId, Dto.Package.UpdatePackageRequestDto request);
        Task<bool> DeleteAsync(int packageId);
    }
}
