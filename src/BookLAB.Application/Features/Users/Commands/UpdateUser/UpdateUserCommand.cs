using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<ResultMessage<UserProfileDto>>
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public bool? IsActive { get; set; }
        public List<int>? Role { get; set; }
        public string? UserCode { get; set; }
    }
}
