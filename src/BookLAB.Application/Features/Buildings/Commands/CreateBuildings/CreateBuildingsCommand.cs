using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Buildings.Commands.CreateBuildings
{
    public class CreateBuildingsCommand : IRequest<PagedList<BuildingDto>>
    {
        public string BuildingName { get; set; }
        public string Descriptions { get; set; }
        public IFormFile Images { get; set; }
    }
}
