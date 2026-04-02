using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BookLAB.Application.Features.LoginWithGoogle
{
    public class LoginWithGoogleHandler : IRequestHandler<LoginWithGoogleCommand, string>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public LoginWithGoogleHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
        {
            var user = new User();
            return _jwtTokenGenerator.GenerateToken(user);
        }
    }
}
