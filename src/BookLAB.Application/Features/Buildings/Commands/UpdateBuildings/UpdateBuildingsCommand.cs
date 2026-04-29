using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Commands.UpdateBuildings
{
    public class UpdateBuildingsCommand : IRequest<PagedList<BuildingDto>>
    {
        public int Id { get; set; }
        public string BuildingName { get; set; }
        public string Descriptions { get; set; }
        public IFormFile Images { get; set; }
    }
}
