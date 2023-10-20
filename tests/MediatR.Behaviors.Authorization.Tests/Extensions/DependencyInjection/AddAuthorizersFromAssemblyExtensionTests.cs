using Microsoft.Extensions.DependencyInjection;
using Xunit;
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;

namespace MediatR.Behaviors.Authorization.Tests.Extensions.DependencyInjection
{
    public class AddAuthorizersFromAssemblyExtensionTests
	{ 
		[Fact(DisplayName = "Should create a valid ISender'")]
		public void Should_create_valid_sender()
		{
            IServiceCollection services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddAuthorizersFromAssemblyExtensionTests).Assembly));
			 
            services.AddAuthorizersFromAssembly(typeof(AddAuthorizersFromAssemblyExtensionTests).Assembly);

            var provider = services.BuildServiceProvider();

            var sender = provider.GetRequiredService<ISender>();

			Assert.NotNull(sender);
        }
	}
}

