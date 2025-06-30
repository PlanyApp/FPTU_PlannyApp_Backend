using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Dto.Gift;
using PlanyApp.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Service.Services
{
    public class GiftService: IGiftService
    {
        private readonly IUnitOfWork _unitOfWork;
        public GiftService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Gift>> GetAllGiftsAsync()
        {
            return await _unitOfWork.GiftRepository.GetAllAsync();
        }
        public async Task<Gift?> GetGiftByIdAsync(int id)
        {
            return await _unitOfWork.GiftRepository.GetByIdAsync(id);
        }

        public async Task<List<Gift>> GetGiftsByUserIdAsync(int userId)
        {
            var userGifts = await _unitOfWork.UserGiftRepository.FindAsync(ug => ug.UserId == userId);

            var giftIds = userGifts.Select(ug => ug.GiftId).ToList();

            var gifts = await _unitOfWork.GiftRepository
                .FindAsync(g => giftIds.Contains(g.GiftId));

            return gifts;
        }
        public async Task<RedeemGiftResponse> RedeemGiftAsync(int userId, int giftId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId)
                       ?? throw new Exception("User not found");

            var gift = await _unitOfWork.GiftRepository.GetByIdAsync(giftId)
                       ?? throw new Exception("Gift not found");

            if (user.Points < gift.Point) 
                throw new Exception("Not enough points to redeem this gift");

            user.Points -= gift.Point;

            var userGift = new UserGift
            {
                UserId = userId,
                GiftId = giftId,
                RedeemedAt = DateTime.UtcNow,
                Code = Guid.NewGuid().ToString("N")[..5]
            };

            await _unitOfWork.UserGiftRepository.AddAsync(userGift);
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return new RedeemGiftResponse
            {
                GiftName = gift.Name,
                Code = userGift.Code,
                RedeemedAt = userGift.RedeemedAt.Value,
                RemainingPoints = user.Points,
            };
        }


    }
}
