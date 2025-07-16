using PlanyApp.Repository.Models;
using PlanyApp.Service.Dto;
using PlanyApp.Service.Dto.Gift;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanyApp.Service.Interfaces
{
    public interface IGiftService
    {
        Task<ICollection<Gift>> GetAllAsync();
        Task<Gift> GetByIdAsync(int id);
        Task<Gift> AddAsync(Gift gift);
        Task<ServiceResponseDto> DeleteAsync(int id);
        Task<RedeemGiftResponse> RedeemGiftAsync(int userId, int giftId);
    }
}
