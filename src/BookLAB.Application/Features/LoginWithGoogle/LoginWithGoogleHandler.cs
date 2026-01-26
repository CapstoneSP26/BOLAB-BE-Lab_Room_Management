using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Persistence;
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

        //private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IAccountRepository _accountRepository;

        private readonly IUnitOfWork _unitOfWork;
        public LoginWithGoogleHandler(IUnitOfWork unitOfWork, IAccountRepository accountRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
        {
            //var googleUser = await _googleAuthService.VerifyTokenAsync(request.IdToken);

            //if (googleUser == null)
            //{
            //    throw new UnauthorizedAccessException("Invalid Google token.");
            //}

            //var user = await _userRepository.GetByEmailAsync(googleUser.Email);
            //if (user == null)
            //{
            //    user = new User
            //    {
            //        Email = googleUser.Email,
            //        FullName = googleUser.Name,
            //        StudentId = null,
            //        IsActive = false,
            //        IsDeleted = false
            //    };
            //    await _userRepository.AddAsync(user);
            //}

            var account = await _accountRepository.GetByProviderUserIdAsync(request.IdToken);
            User user = new User();

            if (account == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.email,
                    FullName = request.fullname,
                    StudentId = request.studentId,
                    IsActive = false,
                    IsDeleted = false
                };

                await _userRepository.AddAsync(user);

                account = new Accounts
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Provider = "Google",
                    ProviderUserId = request.IdToken,
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                await _accountRepository.AddAsync(account);
            }
            else
            {
                account.LastLoginAt = DateTime.UtcNow;
                _accountRepository.Update(account);
            }

            return _jwtTokenGenerator.GenerateToken(user);
        }
    }
}
