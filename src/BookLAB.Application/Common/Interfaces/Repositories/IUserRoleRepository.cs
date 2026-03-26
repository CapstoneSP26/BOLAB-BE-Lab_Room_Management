using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IUserRoleRepository
    {
        public Task<UserRole> GetAsync(Guid userId);
        public Task AddAsync(UserRole userRole);
    }
}
