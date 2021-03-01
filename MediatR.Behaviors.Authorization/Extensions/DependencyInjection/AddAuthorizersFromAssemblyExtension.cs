using MediatR.Behaviors.Authorization.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MediatR.Behaviors.Authorization.Extensions.DependencyInjection
{
    public static class AddAuthorizersFromAssemblyExtension
    {
        public static void AddAuthorizersFromAssembly(
                this IServiceCollection services,
                Assembly assembly,
                ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var authorizerType = typeof(IAuthorizer<>);
            GetTypesAssignableTo(assembly, authorizerType).ForEach((type) =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    if (!implementedInterface.IsGenericType)
                        continue;
                    if (implementedInterface.GetGenericTypeDefinition() != authorizerType)
                        continue;

                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(implementedInterface, type);
                            break;
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(implementedInterface, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(implementedInterface, type);
                            break;
                    }
                }
            });
        }

        private static List<TypeInfo> GetTypesAssignableTo(Assembly assembly, Type compareType)
        {
            var typeInfoList = assembly.DefinedTypes.Where(x => x.IsClass
                                && !x.IsAbstract
                                && x != compareType
                                && x.GetInterfaces()
                                        .Any(i => i.IsGenericType
                                                && i.GetGenericTypeDefinition() == compareType))?.ToList();

            return typeInfoList;
        }
    }
}
