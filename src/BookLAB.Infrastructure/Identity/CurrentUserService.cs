using BookLAB.Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BookLAB.Infrastructure.Identity
{
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
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId);
            }
        }

        public IReadOnlyList<string> Roles => throw new NotImplementedException();

        public bool IsAuthenticated => throw new NotImplementedException();
    }
}
