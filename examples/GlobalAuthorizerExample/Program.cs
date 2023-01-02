using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;
using GlobalAuthorizerExample.Features.GetCourseVideoDetails;
using GlobalAuthorizerExample.Common.Services;
using MediatR.Behaviors.Authorization.Exceptions;
using Newtonsoft.Json;

namespace GlobalAuthorizerExample
{
    public class Program
    {
        public static IServiceProvider Services => _serviceProvider;
        private static readonly IServiceProvider _serviceProvider;
        private static readonly ExampleCurrentUserService _exampleCurrentUserService;

        static Program()
        {
            _exampleCurrentUserService = new ExampleCurrentUserService();
            IServiceCollection services = new ServiceCollection();

            services.AddScoped<ICurrentUserService>(provider => _exampleCurrentUserService);
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddMediatorAuthorization(Assembly.GetExecutingAssembly());
            services.AddAuthorizersFromAssembly(Assembly.GetExecutingAssembly());

            _serviceProvider = services.BuildServiceProvider();
        }

        static async Task Main(string[] args)
        {
            var videoId = new Random().Next(1,1000);
            Console.WriteLine("Welcome! You are currently NOT logged in.");

            try
            {
                Console.WriteLine("Sending request to fetch video details...");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                    await sender.Send(new GetCourseVideoDetails.Request(videoId));
                }
            } catch (UnauthorizedException ex)
            {
                Console.WriteLine($"Unauthorized. Reason: {ex.Message}");
            }
            
            Console.WriteLine("Logging in...");
            _exampleCurrentUserService.SetAuthenticatedUser(1);

            using (var scope = _serviceProvider.CreateScope())
            {
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                Console.WriteLine("Sending request to fetch video details...");
                var response = await sender.Send(new GetCourseVideoDetails.Request(videoId));
                Console.WriteLine(JsonConvert.SerializeObject(response));
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}