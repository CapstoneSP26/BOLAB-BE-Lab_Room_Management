using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly BookLABDbContext _context;

        public UserRoleRepository(BookLABDbContext context)
        {
            _context = context;
        }

        public async Task<UserRole?> GetAsync(Guid userId)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
        }
    }
}
