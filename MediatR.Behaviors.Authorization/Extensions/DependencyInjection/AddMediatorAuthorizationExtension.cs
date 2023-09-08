using MediatR.Behaviors.Authorization.Configuration;
using MediatR.Behaviors.Authorization.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MediatR.Behaviors.Authorization.Extensions.DependencyInjection
{
    public static class AddMediatorAuthorizationExtension
    {
        public static IServiceCollection AddMediatorAuthorization(this IServiceCollection services, Assembly assembly)
        {
            return AddMediatorAuthorization(services, assembly, new AuthorizationPipelineHandlerOptions());
        }

        public static IServiceCollection AddMediatorAuthorization(this IServiceCollection services, Assembly assembly, 
            Action<AuthorizationPipelineHandlerOptions> configuration)
        {
            var options = new AuthorizationPipelineHandlerOptions();
            configuration.Invoke(options);

            return AddMediatorAuthorization(services, assembly, options);
        }

        private static IServiceCollection AddMediatorAuthorization(IServiceCollection services, Assembly assembly, AuthorizationPipelineHandlerOptions options)
        {
            if (options.OnUnauthorized is null)
                throw new InvalidOperationException($"The property {nameof(AuthorizationPipelineHandlerOptions.OnUnauthorized)} " +
                    $"was not assigned in {nameof(AuthorizationPipelineHandlerOptions)}. " +
                    $"You must specify a {nameof(IUnauthorizedResultHandler)}.");

            services.AddTransient(provider => options);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestAuthorizationBehavior<,>));
            AddAuthorizationHandlers(services, assembly);

            return services;
        }

        private static IServiceCollection AddAuthorizationHandlers(IServiceCollection services, Assembly assembly)
        {
            var authHandlerOpenType = typeof(IAuthorizationHandler<>);
            GetTypesAssignableTo(assembly, authHandlerOpenType)
                .ForEach((concretion) => {
                    foreach(var implementedInterface in concretion.ImplementedInterfaces)
                    {
                        if (!implementedInterface.IsGenericType)
                            continue;
                        if (implementedInterface.GetGenericTypeDefinition() != authHandlerOpenType)
                            continue;

                        services.AddTransient(implementedInterface, concretion);
                    }
                });

            return services;
        }

        private static List<TypeInfo> GetTypesAssignableTo(Assembly assembly, Type compareType)
        {
            return assembly.DefinedTypes.Where(x => x.IsClass
                                && !x.IsAbstract
                                && x != compareType
                                && x.GetInterfaces()
                                        .Any(i => i.IsGenericType
                                                && i.GetGenericTypeDefinition() == compareType))?.ToList();
        }


    }
}
