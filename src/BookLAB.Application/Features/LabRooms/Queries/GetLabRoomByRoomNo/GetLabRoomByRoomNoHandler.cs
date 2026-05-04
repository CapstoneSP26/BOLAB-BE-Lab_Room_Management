using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRoomById;
using BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;
using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRoomByRoomNo
{
    internal class GetLabRoomByRoomNoHandler
    {
        public readonly IUnitOfWork _unitOfWork;
        public GetLabRoomByRoomNoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LabRoomDto?> Handle(GetLabRoomByRoomNoQuery request, CancellationToken ct)
        {
            return await _unitOfWork.Repository<LabRoom>().Entities
                .AsNoTracking()
                .Where(x => x.RoomNo == request.RoomNo)
                .SelectLabRoom(false, false, false)
                .FirstOrDefaultAsync(ct);
        }
    }
}
