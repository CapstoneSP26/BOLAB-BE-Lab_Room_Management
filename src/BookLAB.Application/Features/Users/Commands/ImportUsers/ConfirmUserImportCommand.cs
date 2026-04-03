using BookLAB.Application.Features.Users.Common;
using MediatR;

namespace BookLAB.Application.Features.Users.Commands.ImportUsers
{
    public class ConfirmUserImportCommand : IRequest<bool>
    {
        public List<UserImportDto> ValidUsers { get; set; } = new();
    }
}
