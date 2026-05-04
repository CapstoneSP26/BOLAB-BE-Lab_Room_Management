using BookLAB.Application.Features.Groups.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupByName
{
    public class GetGroupByNameQuery : IRequest<List<GroupDto>>
    {
        public string GroupName { get; set; }
    }
}
