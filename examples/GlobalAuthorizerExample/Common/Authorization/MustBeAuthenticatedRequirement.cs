using GlobalAuthorizerExample.Common.Services;
using MediatR.Behaviors.Authorization;

namespace GlobalAuthorizerExample.Common.Authorization
{
    public class MustBeAuthenticatedRequirement : IAuthorizationRequirement
    {
        class MustBeAuthenticatedRequirementHandler : IAuthorizationHandler<MustBeAuthenticatedRequirement>
        {
            private readonly ICurrentUserService _currentUserService;

            public MustBeAuthenticatedRequirementHandler(ICurrentUserService currentUserService)
            {
                _currentUserService = currentUserService;
            }

            public Task<AuthorizationResult> Handle(MustBeAuthenticatedRequirement requirement, CancellationToken cancellationToken = default)
            {
                if (_currentUserService.IsAuthenticated)
                    return Task.FromResult(AuthorizationResult.Succeed());

                return Task.FromResult(AuthorizationResult.Fail("You must be logged in."));
            }
        }
    }
}
