using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Commands.DeleteBuildings
{
    public class DeleteBuildingsCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
