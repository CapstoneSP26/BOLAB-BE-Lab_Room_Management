using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<ResultMessage<UserProfileDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<int>? RoleIds { get; set; } = new();
        public string? UserCode { get; set; }
        public bool? IsActive { get; set; }
        public List<int> Roles { get; set; }
    }
}
