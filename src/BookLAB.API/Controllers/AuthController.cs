using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.LoginWithGoogle;
using BookLAB.Domain.Common;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Persistence;
using BookLAB.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace BookLAB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly LinkGenerator _linkGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public AuthController(IMediator mediator,
                              LinkGenerator linkGenerator,
                              IUserRepository userRepository,
                              IUserRoleRepository userRoleRepository)
        {
            _mediator = mediator;
            _linkGenerator = linkGenerator;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        // front-end sẽ vào đây đầu tiên
        [HttpGet("login/google")]
        public async Task<IResult> GoogleLogin([FromQuery] string returnUrl)
        {
            var redirectUrl = _linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback") + $"?returnUrl={returnUrl}";

            // Tạo AuthenticationProperties thủ công
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Results.Challenge(properties, ["Google"]); // Result.Challenge() chính là thứ yêu cầu đăng nhập bằng bên thứ 3, 
        }

        [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
        public async Task<IResult> GoogleLoginCallback([FromQuery] string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            var account = await _userRepository.GetByProviderUserIdAsync(result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            if (account == null)
            {
                return Results.NotFound();
            }
            var userId = account.Id;

            var role = await _userRoleRepository.GetAsync(userId);

            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();

            var claims = new List<Claim>
                {
                    new Claim("Id", account.Id.ToString()),
                    new Claim("Role", role.RoleId.ToString()),
                };

            var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var signCredential = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

            var preparedToken = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signCredential);

            var generatedToken = new JwtSecurityTokenHandler().WriteToken(preparedToken);

            HttpContext.Response.Cookies.Append("accessToken", generatedToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

            return Results.Redirect($"{returnUrl}");
        }
    }
}