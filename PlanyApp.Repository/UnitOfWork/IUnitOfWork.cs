using PlanyApp.Repository.Base;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<Challenge> ChallengeRepository { get; }
        GenericRepository<Group> GroupRepository { get; }
        GenericRepository<GroupMember> GroupMemberRepository { get; }
        GenericRepository<Hotel> HotelRepository { get; }
        GenericRepository<Image> ImageRepository { get; }
        GenericRepository<Invoice> InvoiceRepository { get; }
        GenericRepository<Item> ItemRepository { get; }
        GenericRepository<Package> PackageRepository { get; }
        GenericRepository<Place> PlaceRepository { get; }
        GenericRepository<Plan> PlanRepository { get; }
        GenericRepository<PlanList> PlanListRepository { get; }
        GenericRepository<Rating> RatingRepository { get; }
        GenericRepository<Role> RoleRepository { get; }
        GenericRepository<Transportation> TransportationRepository { get; }
        IUserRepository UserRepository { get; }
        GenericRepository<UserChallengeProgress> UserChallengeProgressRepository { get; }
        GenericRepository<UserPackage> UserPackageRepository { get; }
        GenericRepository<Gift> GiftRepository { get; }
        GenericRepository<UserGift> UserGiftRepository { get; }
        GenericRepository<Province> ProvinceRepository { get; }
        GenericRepository<User> UserRepo2 { get; }

        int Save();
        Task<int> SaveAsync();

    }
}
