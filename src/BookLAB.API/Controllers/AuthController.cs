using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Users.Commands.UpdateUserProfile;
using BookLAB.Application.Features.Users.Queries.GetCurrentUser;
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
        public async Task<IActionResult> GoogleLoginCallback([FromQuery] string returnUrl)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var providerId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (string.IsNullOrEmpty(providerId))
            {
                return Unauthorized();
            }
            
            var account = await _userRepository.GetByProviderUserIdAsync(providerId);

            // Create user if first-time Google login
            if (account == null)
            {
                var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value ?? "";
                
                // Validate email is from FPT domain
                if (string.IsNullOrEmpty(email) || !email.EndsWith("@fpt.edu.vn", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { error = "Email must be from @fpt.edu.vn domain" });
                }
                
                var fullName = result.Principal?.FindFirst(ClaimTypes.Name)?.Value ?? "Google User";
                
                account = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FullName = fullName,
                    UserCode = email.Split('@')[0], // Use email prefix as user code
                    Provider = "Google",
                    ProviderId = providerId,
                    CampusId = 1, // Default to Main Campus
                    IsActive = true,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UserImageUrl = ""
                };
                
                await _userRepository.AddAsync(account);

                // Create UserRole with default role (Lecturer - RoleId = 1)
                var userRole = new UserRole
                {
                    UserId = account.Id,
                    RoleId = 1 // Lecturer role
                };
                await _userRoleRepository.AddAsync(userRole);
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
        new Claim("Role", role.RoleId.ToString()),
    };

            var secretKey = configuration["JWT:SecretKey"] ?? throw new InvalidOperationException("JWT:SecretKey not found");
            var symetricKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey));
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

            // Token is already in the cookie, now redirect to frontend
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            // Default redirect to dashboard if no returnUrl provided
            return Redirect("https://localhost:5173/labmanager/dashboard");
        }


        [HttpGet("sign-out")]
        public new async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Ok(new
            {
                success = true,
                message = "Sign out successfully!"
            });
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        /// <returns>Current user profile information</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var query = new GetCurrentUserQuery(userId);
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user profile", error = ex.Message });
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        /// <param name="request">Updated profile information</param>
        /// <returns>Updated user profile</returns>
        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentUser(
            [FromBody] UpdateUserProfileRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userIdClaim = User.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var command = new UpdateUserProfileCommand
                {
                    UserId = userId,
                    FullName = request.FullName,
                    UserImageUrl = request.UserImageUrl ?? ""
                };

                var result = await _mediator.Send(command, cancellationToken);

                return Ok(new
                {
                    success = true,
                    message = "Profile updated successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user profile", error = ex.Message });
            }
        }
    }
}