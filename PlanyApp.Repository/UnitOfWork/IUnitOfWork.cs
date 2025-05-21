using PlanyApp.Repository.Base;
using PlanyApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanyApp.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<Category> CategoryRepository { get; }
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
        GenericRepository<User> UserRepository { get; }
        GenericRepository<UserChallengeProgress> UserChallengeProgressRepository { get; }

        int Save();
        Task<int> SaveAsync();

    }
}
