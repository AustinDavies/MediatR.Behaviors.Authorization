using MediatR;

namespace MediatR.Behaviors.Authorization
{
    public interface IAuthorizationRequirement : IRequest<AuthorizationResult>
    {
    }
}
