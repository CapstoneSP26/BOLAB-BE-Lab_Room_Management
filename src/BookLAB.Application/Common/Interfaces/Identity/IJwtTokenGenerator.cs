using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Identity
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
