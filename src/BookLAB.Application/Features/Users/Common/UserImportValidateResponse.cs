
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Users.Common
{
    public class UserImportValidateResponse
    {
        public UserImportMaps maps { get; set; }
        public ImportValidationResult<UserImportDto, User> result { get; set; }

    }
}
