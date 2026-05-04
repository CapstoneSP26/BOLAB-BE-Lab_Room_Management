using AutoMapper;
using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.SlotTypes.GetSlotTypes;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.SlotTypes.Queries.GetSlotTypes;

public class GetSlotTypesQueryHandler : IRequestHandler<GetSlotTypesQuery, List<SlotTypeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSlotTypesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<SlotTypeDto>> Handle(GetSlotTypesQuery request, CancellationToken ct)
    {
        var spec = new SlotTypeSpecification(request);

        var slotTypes = await _unitOfWork.Repository<SlotType>().Entities
            .ApplySpecification(spec)
            .AsNoTracking()
            .ToListAsync(ct);

        return _mapper.Map<List<SlotTypeDto>>(slotTypes);
    }
}