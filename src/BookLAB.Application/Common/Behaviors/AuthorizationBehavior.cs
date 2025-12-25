using MediatR;
using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Security;
using System.Linq;

namespace BookLAB.Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ICurrentUserService _currentUser;

        public AuthorizationBehavior(ICurrentUserService currentUser)
        {
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var authorizeAttributes = request.GetType()
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Cast<AuthorizeAttribute>()
                .ToList();

            // Không yêu cầu quyền → cho qua
            if (!authorizeAttributes.Any())
            {
                return await next();
            }

            if (!_currentUser.IsAuthenticated)
            {
                throw new ForbiddenException("User is not authenticated.");
            }

            foreach (var attribute in authorizeAttributes)
            {
                if (attribute.Roles.Any())
                {
                    //var hasRequiredRole = attribute.Roles
                    //    .Any(role => _currentUser.Roles.Any(r => r == role));
                    var hasRequiredRole = true;

                    if (!hasRequiredRole)
                    {
                        throw new ForbiddenException(
                            $"User does not have required role(s): {string.Join(", ", attribute.Roles)}"
                        );
                    }
                }
            }

            return await next();
        }
    }
}
