using PlanyApp.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PlanyApp.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task<User?> GetByPasswordResetTokenAsync(string token);
        Task<IEnumerable<User>> GetAllAsync();
        Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
        Task<bool> EmailExistsAsync(string email);
    }
} 