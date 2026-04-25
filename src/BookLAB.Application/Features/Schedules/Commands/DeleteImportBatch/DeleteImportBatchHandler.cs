using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Schedules.Commands.DeleteImportBatch
{
    public class DeleteImportBatchHandler : IRequestHandler<DeleteImportBatchCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteImportBatchHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteImportBatchCommand request, CancellationToken cancellationToken)
        {
            var importBatch = await _unitOfWork.Repository<ImportBatch>().GetByIdAsync(request.Id);
            if (importBatch == null)
            {
                return false;
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Tìm tất cả Schedule thuộc Batch này
                var schedules = await _unitOfWork.Repository<Schedule>().Entities
                    .Where(s => s.ImportBatchId == request.Id)
                    .ToListAsync(cancellationToken);

                if (schedules.Any())
                {
                    // 2. Xóa các Schedule liên quan
                    // Nếu dùng Soft Delete: foreach(var s in schedules) s.IsDeleted = true;
                    _unitOfWork.Repository<Schedule>().DeleteRange(schedules);
                }

                // 3. Xóa chính cái Batch đó
                _unitOfWork.Repository<ImportBatch>().Delete(importBatch);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}