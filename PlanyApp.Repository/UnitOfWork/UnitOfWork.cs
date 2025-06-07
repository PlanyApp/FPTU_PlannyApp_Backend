using System;
using System.Threading.Tasks;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.Base;

namespace PlanyApp.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PlanyDbContext _context;

        // Private fields cho từng repository
        private GenericRepository<Category> _categoryRepository;
        private GenericRepository<Challenge> _challengeRepository;
        private GenericRepository<Group> _groupRepository;
        private GenericRepository<GroupMember> _groupMemberRepository;
        private GenericRepository<Hotel> _hotelRepository;
        private GenericRepository<Image> _imageRepository;
        private GenericRepository<Invoice> _invoiceRepository;
        private GenericRepository<Item> _itemRepository;
        private GenericRepository<Package> _packageRepository;
        private GenericRepository<Place> _placeRepository;
        private GenericRepository<Plan> _planRepository;
        private GenericRepository<PlanList> _planListRepository;
        private GenericRepository<Rating> _ratingRepository;
        private GenericRepository<Role> _roleRepository;
        private GenericRepository<Transportation> _transportationRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<UserChallengeProgress> _userChallengeProgressRepository;

        public UnitOfWork(PlanyDbContext context)
        {
            _context = context;
        }

        // Public properties triển khai IUnitOfWork
        public GenericRepository<Category> CategoryRepository => _categoryRepository ??= new GenericRepository<Category>(_context);
        public GenericRepository<Challenge> ChallengeRepository => _challengeRepository ??= new GenericRepository<Challenge>(_context);
        public GenericRepository<Group> GroupRepository => _groupRepository ??= new GenericRepository<Group>(_context);
        public GenericRepository<GroupMember> GroupMemberRepository => _groupMemberRepository ??= new GenericRepository<GroupMember>(_context);
        public GenericRepository<Hotel> HotelRepository => _hotelRepository ??= new GenericRepository<Hotel>(_context);
        public GenericRepository<Image> ImageRepository => _imageRepository ??= new GenericRepository<Image>(_context);
        public GenericRepository<Invoice> InvoiceRepository => _invoiceRepository ??= new GenericRepository<Invoice>(_context);
        public GenericRepository<Item> ItemRepository => _itemRepository ??= new GenericRepository<Item>(_context);
        public GenericRepository<Package> PackageRepository => _packageRepository ??= new GenericRepository<Package>(_context);
        public GenericRepository<Place> PlaceRepository => _placeRepository ??= new GenericRepository<Place>(_context);
        public GenericRepository<Plan> PlanRepository => _planRepository ??= new GenericRepository<Plan>(_context);
        public GenericRepository<PlanList> PlanListRepository => _planListRepository ??= new GenericRepository<PlanList>(_context);
        public GenericRepository<Rating> RatingRepository => _ratingRepository ??= new GenericRepository<Rating>(_context);
        public GenericRepository<Role> RoleRepository => _roleRepository ??= new GenericRepository<Role>(_context);
        public GenericRepository<Transportation> TransportationRepository => _transportationRepository ??= new GenericRepository<Transportation>(_context);
        public GenericRepository<User> UserRepository => _userRepository ??= new GenericRepository<User>(_context);
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
