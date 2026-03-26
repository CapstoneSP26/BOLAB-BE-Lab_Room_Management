using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);
        Task<User?> GetByIdWithRolesAsync(Guid userId);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User?> GetByProviderUserIdAsync(string providerId);

        Task<bool> IfExisted(Guid usreId);
    }
}
