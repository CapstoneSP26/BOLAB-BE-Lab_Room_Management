using BookLAB.Application.Features.SlotTypes.GetSlotTypes;
using MediatR;

namespace BookLAB.Application.Features.SlotTypes.Queries.GetSlotTypes;

public record GetSlotTypesQuery : IRequest<List<SlotTypeDto>>
{
    public int? CampusId { get; set; }
}