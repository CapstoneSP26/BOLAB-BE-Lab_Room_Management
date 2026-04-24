using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<ResultMessage<bool>>
    {
        public Guid Id { get; set; }
    }
}
