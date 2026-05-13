using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.TabletAccounts.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BookLAB.Application.Features.TabletAccounts.Queries
{
    public class LoginTabletHandler : IRequestHandler<LoginTabletQuery, ResultMessage<LoginTabletReturn>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public LoginTabletHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ResultMessage<LoginTabletReturn>> Handle(LoginTabletQuery request, CancellationToken cancellationToken)
        {
            var hashPassword = ComputeHash(request.Password, SHA256.Create());
            var tabletAccount = await _unitOfWork.Repository<TabletAccount>().Entities
                .Include(x => x.LabRoom)
                .ThenInclude(x => x.Building)
                .FirstOrDefaultAsync(x => x.Name == request.Name && x.Password == hashPassword, cancellationToken);

            if (tabletAccount == null) 
                return new ResultMessage<LoginTabletReturn>
                {
                    Success = false,
                    Message = "Account not found",
                    Data = new LoginTabletReturn
                    {
                        ReturnUrl = $"{_configuration["FrontendUrl"]}/login?error=Account_not_found"
                    }
                };

            if (tabletAccount.LabRoom.IsActive != true || tabletAccount.LabRoom.IsDeleted != false)
                return new ResultMessage<LoginTabletReturn>
                {
                    Success = false,
                    Message = "Account locked",
                    Data = new LoginTabletReturn
                    {
                        ReturnUrl = $"{_configuration["FrontendUrl"]}/login?error=Account_locked"
                    }
                };


            var returnUrl = "calendar/" + tabletAccount.LabRoom.RoomNo;
            var userId = tabletAccount.Id;
            var role = 0;

            var claims = new List<Claim>
            {
                new Claim("Id", userId.ToString()),
                new Claim("Role", role.ToString() ?? ""),
                new Claim("CampusId", tabletAccount.LabRoom.Building.CampusId.ToString())
            };

            var symetricKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var signCredential = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

            var preparedToken = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signCredential);

            var generatedToken = new JwtSecurityTokenHandler().WriteToken(preparedToken);

            //HttpContext.Response.Cookies.Append("accessToken", generatedToken,
            //    new CookieOptions
            //    {
            //        Expires = DateTimeOffset.UtcNow.AddMinutes(30),
            //        HttpOnly = true,
            //        IsEssential = true,
            //        Secure = true,
            //        SameSite = SameSiteMode.None
            //    });

            return new ResultMessage<LoginTabletReturn>
            {
                Success = true,
                Message = "Login successful",
                Data = new LoginTabletReturn
                {
                    Token = generatedToken,
                    ReturnUrl = returnUrl
                }
            };
        }

        public string ComputeHash(string input, HashAlgorithm algorithm)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }

    }
}
