using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly BookLABDbContext _context;

        public UserRepository(BookLABDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            if (user == null || user.Id == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<User?> GetByIdWithRolesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.Campus)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByProviderUserIdAsync(string providerId)
        {
            if (providerId == null)
            {
                throw new ArgumentNullException(nameof(providerId));
            }

            return await _context.Users
                .FirstOrDefaultAsync(u => u.ProviderId == providerId && !u.IsDeleted);
        }

        public async Task<bool> IfExisted(Guid usreId)
        {
            return await _context.Users.AnyAsync(u => u.Id == usreId && !u.IsDeleted && u.IsActive);
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null || user.Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}