using Microsoft.EntityFrameworkCore;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace PlanyApp.Repository.Repositories // New folder
{
    public class UserRepository : IUserRepository
    {
        private readonly PlanyDBContext _context;

        public UserRepository(PlanyDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _context.Users.Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Role)
                                     .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByPasswordResetTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Include(u => u.Role)
                                     .Where(predicate)
                                     .ToListAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
} 