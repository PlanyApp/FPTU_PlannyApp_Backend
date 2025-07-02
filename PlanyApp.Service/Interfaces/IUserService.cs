using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IUserService
    {
        Task<int?> GetUserPointsAsync(int userId);

    }
}
