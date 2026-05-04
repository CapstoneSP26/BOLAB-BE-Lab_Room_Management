using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Common;
using MediatR;

namespace BookLAB.Application.Features.Users.Commands.ImportUsers
{
    public class ConfirmUserImportCommand : IRequest<ImportResult>
    {
        public List<UserImportDto> Users { get; set; } = new();
        public int CampusId { get; set; }
    }
}
