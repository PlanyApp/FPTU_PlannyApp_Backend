using System;
using System.Threading.Tasks;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.Base;
using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Repositories;


namespace PlanyApp.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PlanyDbContext _context;

        private GenericRepository<Challenge> _challengeRepository = null!;
        private GenericRepository<Group> _groupRepository = null!;
        private GenericRepository<GroupMember> _groupMemberRepository = null!;
        private GenericRepository<Hotel> _hotelRepository = null!;
        private GenericRepository<Image> _imageRepository = null!;
        private GenericRepository<ImageS3> _imageS3Repository = null!;
        private GenericRepository<Invoice> _invoiceRepository = null!;
        private GenericRepository<Item> _itemRepository = null!;
        private GenericRepository<Package> _packageRepository = null!;
        private GenericRepository<Place> _placeRepository = null!;
        private GenericRepository<Plan> _planRepository = null!;
        private GenericRepository<PlanList> _planListRepository = null!;
        private GenericRepository<Rating> _ratingRepository = null!;
        private GenericRepository<Role> _roleRepository = null!;
        private GenericRepository<Transportation> _transportationRepository = null!;
        private IUserRepository _userRepository = null!;
        private GenericRepository<UserChallengeProgress> _userChallengeProgressRepository = null!;

        public UnitOfWork(PlanyDbContext context)
        {
            _context = context;
            // Fields are initialized by their respective properties on first access or can be explicitly set to null! here
        }

        public GenericRepository<Challenge> ChallengeRepository => _challengeRepository ??= new GenericRepository<Challenge>(_context);
        public GenericRepository<Group> GroupRepository => _groupRepository ??= new GenericRepository<Group>(_context);
        public GenericRepository<GroupMember> GroupMemberRepository => _groupMemberRepository ??= new GenericRepository<GroupMember>(_context);
        public GenericRepository<Hotel> HotelRepository => _hotelRepository ??= new GenericRepository<Hotel>(_context);
        public GenericRepository<Image> ImageRepository => _imageRepository ??= new GenericRepository<Image>(_context);
        public GenericRepository<ImageS3> ImageS3Repository => _imageS3Repository ??= new GenericRepository<ImageS3>(_context);
        public GenericRepository<Invoice> InvoiceRepository => _invoiceRepository ??= new GenericRepository<Invoice>(_context);
        public GenericRepository<Item> ItemRepository => _itemRepository ??= new GenericRepository<Item>(_context);
        public GenericRepository<Package> PackageRepository => _packageRepository ??= new GenericRepository<Package>(_context);
        public GenericRepository<Place> PlaceRepository => _placeRepository ??= new GenericRepository<Place>(_context);
        public GenericRepository<Plan> PlanRepository => _planRepository ??= new GenericRepository<Plan>(_context);
        public GenericRepository<PlanList> PlanListRepository => _planListRepository ??= new GenericRepository<PlanList>(_context);
        public GenericRepository<Rating> RatingRepository => _ratingRepository ??= new GenericRepository<Rating>(_context);
        public GenericRepository<Role> RoleRepository => _roleRepository ??= new GenericRepository<Role>(_context);
        public GenericRepository<Transportation> TransportationRepository => _transportationRepository ??= new GenericRepository<Transportation>(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public GenericRepository<UserChallengeProgress> UserChallengeProgressRepository => _userChallengeProgressRepository ??= new GenericRepository<UserChallengeProgress>(_context);

        // Save & SaveAsync
        public int Save() => _context.SaveChanges();
        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        // IDisposable
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
