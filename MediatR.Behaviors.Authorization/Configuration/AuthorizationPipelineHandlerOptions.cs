using MediatR.Behaviors.Authorization.Interfaces;

namespace MediatR.Behaviors.Authorization.Configuration
{
    public class AuthorizationPipelineHandlerOptions
    {
        public IUnauthorizedResultHandler OnUnauthorized { get; private set; } = new DefaultUnauthorizedHandlerStrategy();

        public AuthorizationPipelineHandlerOptions UseUnauthorizedResultHandlerStrategy(IUnauthorizedResultHandler strategy)
        {
            OnUnauthorized = strategy;

            return this;
        }
    }
}
