using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<List<UserProfileDto>>
    {
        public string? q { get; set; } = "";
        public string? keyword { get; set; } = "";
        public string? searchTerm { get; set; } = "";
        public string? UserCode { get; set; } = "";
        public string? role { get; set; }
    }
}
