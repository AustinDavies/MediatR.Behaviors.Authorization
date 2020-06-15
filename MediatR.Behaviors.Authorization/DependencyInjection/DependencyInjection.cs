using MediatR.Behaviors.Authorization.Extensions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatR.Behaviors.Authorization.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMediatorAuthorization(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestAuthorizationBehavior<,>));
            services.AddAuthorizersFromAssembly(assembly, lifetime);

            return services;
        }
    }
}
