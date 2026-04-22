using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Auth.Queries.GetProfile;
using BookLAB.Application.Features.LoginWithGoogle;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IMediator mediator,
                              LinkGenerator linkGenerator,
                              IUserRepository userRepository,
                              IUserRoleRepository userRoleRepository,
                              IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _linkGenerator = linkGenerator;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _unitOfWork = unitOfWork;
        }

        // front-end sẽ vào đây đầu tiên
        [HttpGet("login/google")]
        public async Task<IActionResult> GoogleLogin([FromQuery] string returnUrl)
        {
            var redirectUrl = _linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback") + $"?returnUrl={returnUrl}";

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            // Challenge sẽ yêu cầu xác thực bằng Google
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallback([FromQuery] string returnUrl, CancellationToken cancellationToken)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var account = await _userRepository.GetByEmailAsync(result.Principal?.FindFirst(ClaimTypes.Email)?.Value);

            if (account == null)
            {
                return NotFound();
            }

            var userId = account.Id;
            var role = await _userRoleRepository.GetAsync(userId);

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var claims = new List<Claim>
            {
                new Claim("Id", account.Id.ToString()),
                new Claim("Role", role?.RoleId.ToString() ?? ""),
                new Claim("CampusId", account.CampusId.ToString())
            };

            var symetricKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
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

            switch (role.RoleId)
            {
                case 1:
                    returnUrl = "https://localhost:5173/labmanager/dashboard";
                    break;
                case 2:
                    returnUrl = "https://localhost:5173/labmanager/dashboard";
                    break;
                case 3:
                    returnUrl = "https://localhost:5173/";
                    break;
                case 4:
                    returnUrl = "https://localhost:5173/";
                    break;
                default:
                    returnUrl = "https://localhost:5173/";
                    break;
            }

            var pictureUrl = result.Principal?.FindFirst("picture")?.Value;
            Console.WriteLine($"Url: {pictureUrl}");

            if (pictureUrl != null && account.UserImageUrl != pictureUrl)
            {
                account.UserImageUrl = pictureUrl;
                try
                {
                    await _unitOfWork.BeginTransactionAsync();
                    await _unitOfWork.Repository<User>().UpdateAsync(account);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync();
                } catch (Exception ex)
                {
                    Console.WriteLine("Update user failed: " + ex.ToString());
                }
                
            }

            // Validate returnUrl to avoid invalid redirect targets.
            //if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var parsedReturnUrl)
            //    || (parsedReturnUrl.Scheme != Uri.UriSchemeHttp && parsedReturnUrl.Scheme != Uri.UriSchemeHttps))
            //{
            //    returnUrl = "https://localhost:5173/";
            //}

            return Redirect(returnUrl);
        }

        [HttpGet("sign-out")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync("Cookies");

            HttpContext.Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return Ok(new
            {
                success = true,
                message = "Sign out successfully!"
            });
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var result = await _mediator.Send(new GetProfileQuery());
            return Ok(result);
        }
    }
}