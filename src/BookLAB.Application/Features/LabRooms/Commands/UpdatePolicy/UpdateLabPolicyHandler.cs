using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace BookLAB.Application.Features.LabRooms.Commands.UpdatePolicy
{
    public class UpdateLabPolicyHandler : IRequestHandler<UpdateLabPolicyCommand, LabRoomPolicyUpdateDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLabPolicyHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LabRoomPolicyUpdateDto> Handle(UpdateLabPolicyCommand request, CancellationToken ct)
        {
            // 1. Tìm chính sách hiện tại
            var policy = await _unitOfWork.Repository<RoomPolicy>().Entities
                .FirstOrDefaultAsync(x => x.LabRoomId == request.LabRoomId
                                     && x.PolicyKey == request.PolicyKey, ct);

            if (policy == null)
            {
                // Nếu không tìm thấy, tạo mới (Logic Upsert)
                policy = new RoomPolicy
                {
                    LabRoomId = request.LabRoomId,
                    PolicyKey = request.PolicyKey,
                    PolicyValue = request.PolicyValue ?? "",
                    IsActive = request.IsActive ?? true
                };
                await _unitOfWork.Repository<RoomPolicy>().AddAsync(policy);
            }
            else
            {
                // 2. Nếu tìm thấy, cập nhật các trường được gửi lên (Partial Update)
                if (request.PolicyValue != null) policy.PolicyValue = request.PolicyValue;
                if (request.IsActive.HasValue) policy.IsActive = request.IsActive.Value;

                _unitOfWork.Repository<RoomPolicy>().Update(policy);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return new LabRoomPolicyUpdateDto
            {
                LabRoomId = policy.LabRoomId,
                PolicyKey = policy.PolicyKey.ToString(),
                PolicyValue = policy.PolicyValue,
                IsActive = policy.IsActive
            };
        }
    }
}
