using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Behaviors.Authorization.Extensions.DependencyInjection
{
    public static class AddMediatorAuthorizationExtension
    {
        public static IServiceCollection AddMediatorAuthorization(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestAuthorizationBehavior<,>));
            return services;
        }
    }
}
