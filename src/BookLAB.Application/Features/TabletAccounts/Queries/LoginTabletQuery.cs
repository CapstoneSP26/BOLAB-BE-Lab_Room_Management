using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.TabletAccounts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.TabletAccounts.Queries
{
    public class LoginTabletQuery : IRequest<ResultMessage<LoginTabletReturn>>
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
