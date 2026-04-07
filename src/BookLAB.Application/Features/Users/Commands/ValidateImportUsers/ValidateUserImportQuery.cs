using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Common;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Users.Commands.ValidateImportUsers
{
    public class ValidateUserImportQuery : IRequest<ImportValidationResult<UserImportDto, User>>
    {
        public List<UserImportDto> Users { get; set; } = new();
        public int CampusId { get; set; }

    }
}
