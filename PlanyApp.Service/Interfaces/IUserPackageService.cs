using PlanyApp.Service.Dto.UserPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface  IUserPackageService
    {
        // get ckage by user id
        Task<List<ResponseListUserPackage>> GetPackageIdsByUserIdAsync(int userId);
    }
}
