using MediatR;

namespace BookLAB.Application.Features.Buildings.Commands.DeleteBuilding
{
    public class DeleteBuildingCommand : IRequest
    {
        public int Id { get; set; }
    }
}
