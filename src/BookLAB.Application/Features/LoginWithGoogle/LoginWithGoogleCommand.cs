using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LoginWithGoogle
{
    public record LoginWithGoogleCommand(string IdToken, string email, string fullname, string? studentId) : IRequest<string>;
}
