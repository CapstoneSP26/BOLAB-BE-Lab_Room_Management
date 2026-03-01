using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Common;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        public async Task<GoogleUser?> VerifyTokenAsync(string idToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
            return new GoogleUser
            {
                Email = payload.Email,
                Name = payload.Name,
                GivenName = payload.GivenName,
                NameIdentifier = payload.Subject
            };
        }
    }
}
