using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.DeleteImportBatch
{
    public class DeleteImportBatchCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
