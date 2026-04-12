using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Groups.DTOs;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Groups.Queries.ValidateGroupImport
{
    public class ValidateGroupImportQuery : IRequest<ImportValidationResult<GroupImportDto, GroupMember>>
    {
        public List<GroupImportDto> Groups { get; set; } = new();
        public int CampusId { get; set; }
    }
}