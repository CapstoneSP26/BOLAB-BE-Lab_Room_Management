using BookLAB.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleUser?> VerifyTokenAsync(string idToken);
    }
}
