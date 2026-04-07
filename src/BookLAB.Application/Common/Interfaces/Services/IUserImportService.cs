using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IUserImportService
    {
        Task<UserImportValidateResponse> ValidateAsync(
            List<UserImportDto> users,
            int campusId,
            CancellationToken cancellationToken,
            bool isAllowCreateImportData = false);
    }
}
