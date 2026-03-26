using BookLAB.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Claims;
using BookLAB.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Http;

namespace BookLAB.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            // First try to find the "Id" claim (set by AuthController)
            var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("Id");
            
            // Fallback to ClaimTypes.NameIdentifier if "Id" not found
            if (idClaim == null)
            {
                idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            }
            
            if (idClaim == null) return null;
            
            return Guid.TryParse(idClaim.Value, out var userId) ? userId : null;
        }
    }

    public IReadOnlyList<string> Roles 
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList() ?? new List<string>();
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
