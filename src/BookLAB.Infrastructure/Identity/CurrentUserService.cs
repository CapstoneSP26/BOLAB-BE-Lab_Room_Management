using BookLAB.Application.Common.Interfaces.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLAB.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid? UserId => throw new NotImplementedException();

        public IReadOnlyList<string> Roles => throw new NotImplementedException();

        public bool IsAuthenticated => throw new NotImplementedException();
    }
}
