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
    }
}
