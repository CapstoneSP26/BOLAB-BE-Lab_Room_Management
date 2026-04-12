using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRoomById;

public class GetLabRoomByIdQueryHandler : IRequestHandler<GetLabRoomByIdQuery, LabRoomDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLabRoomByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LabRoomDto?> Handle(GetLabRoomByIdQuery request, CancellationToken ct)
    {
        return await _unitOfWork.Repository<LabRoom>().Entities
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .SelectLabRoom(request.IncludeImages, request.IncludeBuilding)
            .FirstOrDefaultAsync(ct);
    }
}
