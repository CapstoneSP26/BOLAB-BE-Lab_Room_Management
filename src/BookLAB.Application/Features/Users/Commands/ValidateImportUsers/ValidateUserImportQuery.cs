using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Common;
using MediatR;

namespace BookLAB.Application.Features.Users.Commands.ValidateImportUsers
{
    public class ValidateUserImportQuery : IRequest<ImportValidationResult<UserImportDto>>
    {
        public List<UserImportDto> Users { get; set; } = new();
    }
}
