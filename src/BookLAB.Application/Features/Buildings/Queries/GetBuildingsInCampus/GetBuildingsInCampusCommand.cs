using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildingsInCampus
{
    public class GetBuildingsInCampusCommand : IRequest<List<BuildingResponse>>
    {
        public int campusId { get; set; }
    }
}
