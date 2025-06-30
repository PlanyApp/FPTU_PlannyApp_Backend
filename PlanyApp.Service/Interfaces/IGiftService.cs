using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto.Gift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IGiftService
    {
        Task<List<Gift>> GetAllGiftsAsync();
        Task<Gift?> GetGiftByIdAsync(int id);
        Task<List<Gift>> GetGiftsByUserIdAsync(int userId);
        Task<RedeemGiftResponse> RedeemGiftAsync(int userId, int giftId);


    }
}
