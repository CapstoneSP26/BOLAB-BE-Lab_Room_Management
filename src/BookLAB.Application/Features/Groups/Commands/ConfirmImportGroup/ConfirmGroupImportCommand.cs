using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Groups.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.ConfirmImportGroup
{
    public class ConfirmGroupImportCommand : IRequest<ImportResult>
    {
        public List<GroupImportDto> Groups { get; set; } = new();
        public int CampusId { get; set; }
    }
}
