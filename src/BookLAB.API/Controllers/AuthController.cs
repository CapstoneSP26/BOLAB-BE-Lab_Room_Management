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
        private readonly BookLABDbContext _context;
        private readonly LinkGenerator _linkGenerator;
        //private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;

        public AuthController(IMediator mediator, 
                              BookLABDbContext context,
                              LinkGenerator linkGenerator,
                              //SignInManager<User> signInManager,
                              IUserRepository userRepository,
                              IUserRoleRepository userRoleRepository)
        {
            _mediator = mediator;
            _context = context;
            _linkGenerator = linkGenerator;
            //_signInManager = signInManager;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] LoginWithGoogleCommand command)
        {
            var token = await _mediator.Send(command);
            return Ok(new { Token = token });
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            LoginWithGoogleCommand command = new LoginWithGoogleCommand(claims.FirstOrDefault(a => Regex.IsMatch(a.Type, @"/nameidentifier$")).Value,
                                                                        claims.FirstOrDefault(a => Regex.IsMatch(a.Type, @"/emailaddress$")).Value,
                                                                        claims.FirstOrDefault(a => Regex.IsMatch(a.Type, @"/name$")).Value,
                                                                        null);
            var token = await _mediator.Send(command);

            return Ok(token);

            //return RedirectToAction("Index", "Home", new {area= ""} );
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        // front-end sẽ vào đây đầu tiên
        [HttpGet("login/google")]
        public async Task<IResult> GoogleLogin([FromQuery] string returnUrl, string? studentId)
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
        public async Task<IResult> GoogleLoginCallback([FromQuery] string returnUrl, string? studentId)
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Results.Unauthorized();
            }

            var account = await _accountRepository.GetByProviderUserIdAsync(result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userId = Guid.NewGuid();

            if (account == null)
            {
                User user = new User
                {
                    Id = userId,
                    Email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value,
                    FullName = result.Principal?.FindFirst(ClaimTypes.Name)?.Value,
                    StudentId = studentId,
                    IsActive = false,
                    IsDeleted = false
                };

                await _userRepository.AddAsync(user);

                Accounts newAccount = new Accounts
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    LastLoginAt = DateTime.Now,
                    PasswordHash = null,
                    Provider = "Google",
                    ProviderUserId = result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                };

                await _accountRepository.AddAsync(newAccount);
            } else
            {
                account.LastLoginAt = DateTime.Now;
                _accountRepository.Update(account);
            }

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