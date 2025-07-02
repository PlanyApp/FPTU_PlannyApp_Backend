using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class UserService: IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService( IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int?> GetUserPointsAsync(int userId)
        {
            var user = await _unitOfWork.UserRepo2.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return user.Points ?? 0;
        }

    }
}
