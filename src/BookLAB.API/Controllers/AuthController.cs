using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Auth.Queries.GetProfile;
using BookLAB.Application.Features.LoginWithGoogle;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IMediator mediator,
                              LinkGenerator linkGenerator,
                              IUserRepository userRepository,
                              IUserRoleRepository userRoleRepository,
                              IUnitOfWork unitOfWork,
                              IConfiguration configuration,
                              ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _linkGenerator = linkGenerator;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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
                return Redirect($"{_configuration["FrontendUrl"]}/login?error=User_not_found");

            if (account.IsActive != true || account.IsDeleted != false)
                return Redirect($"{_configuration["FrontendUrl"]}/login?error=Account_locked");

            var userId = account.Id;
            //var role = await _userRoleRepository.GetAsync(userId);
            var role = await _unitOfWork.Repository<UserRole>().Entities.Where(x => x.UserId == userId).MinAsync(x => x.RoleId);

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var claims = new List<Claim>
            {
                new Claim("Id", account.Id.ToString()),
                new Claim("Role", role.ToString() ?? ""),
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

            switch (role)
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
                    returnUrl = "https://localhost:5173/student";
                    break;
                default:
                    returnUrl = "https://localhost:5173/";
                    break;
            }

            var pictureUrl = result.Principal?.FindFirst("picture")?.Value;
            var providerId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"Url: {pictureUrl}");

            if (pictureUrl != null && account.UserImageUrl != pictureUrl)
            {
                account.UserImageUrl = pictureUrl;

                if (providerId != null && account.ProviderId != providerId)
                {
                    account.ProviderId = providerId;
                    account.Provider = "Google";
                }

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

            return Redirect(returnUrl);
        }

        [HttpPost("change-role/{roleId}")]
        [Authorize]
        public async Task<IActionResult> ChangeRole([FromRoute] int roleId)
        {
            var userId = _currentUserService.UserId;
            var roles = await _unitOfWork.Repository<UserRole>().Entities
                .Where(r => r.UserId == userId)
                .Select(x => x.RoleId)
                .ToListAsync();
            var campusId = _currentUserService.CampusId;

            if (!roles.Contains(roleId))
                return BadRequest(new ResultMessage<bool>
                {
                    Success = false,
                    Message = "You don't have this role!"
                });

            // ❌ Xóa: await HttpContext.SignOutAsync("Cookies");

            // Xóa cookie cũ
            HttpContext.Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            // Tạo token mới
            var claims = new List<Claim>
            {
                new Claim("Id", userId.ToString()),
                new Claim("Role", roleId.ToString()),
                new Claim("CampusId", campusId.ToString())
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

            // Append token mới
            HttpContext.Response.Cookies.Append("accessToken", generatedToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

            // ✅ Trả JSON thay vì Redirect
            var redirectUrl = roleId switch
            {
                1 => "/labmanager/dashboard",    // Admin
                2 => "/labmanager/dashboard",    // Lab Manager
                3 => "/",                        // Lecturer
                4 => "/student",                 // Student
                _ => "/"
            };

            return Ok(new
            {
                success = true,
                redirectUrl,
                message = $"Role switched successfully"
            });
        }

        [HttpGet("user-roles")]
        public async Task<IActionResult> GetUserRoles()
        {
            var userId = _currentUserService.UserId;
            var roles = await _unitOfWork.Repository<UserRole>().Entities.Where(r => r.UserId == userId).Select(x => x.RoleId).ToListAsync();
            return Ok(roles);
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