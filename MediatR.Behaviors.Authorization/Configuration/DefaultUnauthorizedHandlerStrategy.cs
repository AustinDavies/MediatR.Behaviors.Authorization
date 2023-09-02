using MediatR.Behaviors.Authorization.Exceptions;
using MediatR.Behaviors.Authorization.Interfaces;
using System.Threading.Tasks;

namespace MediatR.Behaviors.Authorization.Configuration
{
    internal class DefaultUnauthorizedHandlerStrategy : IUnauthorizedResultHandler
    {            
        public Task<TResponse> Invoke<TResponse>(AuthorizationResult result)
            => throw new UnauthorizedException(result.FailureMessage);
    }
}
