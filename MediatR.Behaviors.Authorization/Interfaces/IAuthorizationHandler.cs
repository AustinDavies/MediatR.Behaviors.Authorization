using MediatR;

namespace MediatR.Behaviors.Authorization
{
    public interface IAuthorizationHandler<TRequest> : IRequestHandler<TRequest, AuthorizationResult>
        where TRequest : IRequest<AuthorizationResult>
    {

    }
}
