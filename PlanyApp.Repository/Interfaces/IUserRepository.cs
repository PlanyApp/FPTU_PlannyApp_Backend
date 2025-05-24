using PlanyApp.Repository.Models;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByGoogleIdAsync(string googleId);
        Task<User> GetByPasswordResetTokenAsync(string token);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
} 