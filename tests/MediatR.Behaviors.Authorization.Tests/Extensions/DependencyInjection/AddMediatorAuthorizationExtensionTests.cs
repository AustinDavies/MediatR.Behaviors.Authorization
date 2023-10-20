using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Behaviors.Authorization.Tests.Extensions.DependencyInjection
{
    public class AddMediatorAuthorizationExtensionTests
	{ 

        [Fact(DisplayName = "Should create a valid ISender' when use AddMediatorAuthorization()'")]
        public void Should_create_valid_sender()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddAuthorizersFromAssemblyExtensionTests).Assembly));

            services.AddMediatorAuthorization(typeof(AddAuthorizersFromAssemblyExtensionTests).Assembly);

            var provider = services.BuildServiceProvider();

            var sender = provider.GetRequiredService<ISender>();

            Assert.NotNull(sender);
        }
    }
}

