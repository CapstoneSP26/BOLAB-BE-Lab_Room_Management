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
        public Guid Email { get; set; }
        public Guid FullName { get; set; }
        public Guid IsActive { get; set; }
        public Guid Role { get; set; }
        public Guid UserCode { get; set; }
    }
}
