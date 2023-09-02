# MediatR.Behaviors.Authorization

[![NuGet](https://img.shields.io/nuget/v/MediatR.Behaviors.Authorization.svg)](https://www.nuget.org/packages/MediatR.Behaviors.Authorization/)

A simple request authorization package that allows you to build and run request specific authorization requirements before your request handler is called. You can read my [article][article] on the reasoning for this library for further analysis.

## Installation

Using the [.NET Core command-line interface (CLI) tools][dotnet-core-cli-tools]:

```sh
dotnet add package MediatR.Behaviors.Authorization
```

Using the [NuGet Command Line Interface (CLI)][nuget-cli]:

```sh
nuget install MediatR.Behaviors.Authorization
```

Using the [Package Manager Console][package-manager-console]:

```powershell
Install-Package MediatR.Behaviors.Authorization
```

From within Visual Studio:

1. Open the Solution Explorer.
2. Right-click on a project within your solution.
3. Click on *Manage NuGet Packages...*
4. Click on the *Browse* tab and search for "MediatR.Behaviors.Authorization".
5. Click on the MediatR.Behaviors.Authorization package, select the latest version in the
   right-tab and click *Install*.


## Getting Started

### Dependency Injection

You will need to register the authorization pipeline along with all implementations of `IAuthorizer`:

```c#
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;

public class Startup
{
	//...
	public void ConfigureServices(IServiceCollection services)
	{
		// Adds the transient pipeline behavior and additionally registers all `IAuthorizationHandlers` for a given assembly
		services.AddMediatorAuthorization(Assembly.GetExecutingAssembly());
		// Register all `IAuthorizer` implementations for a given assembly
		services.AddAuthorizersFromAssembly(Assembly.GetExecutingAssembly())

	}
}
```
You can use the helper method to register 'IAuthorizer' implementations from an assembly or manually inject them using Microsoft's DI methods.

## Example Usage

Scenario: We need to get details about a specific video for a course on behalf of a user. However, this video course information is considered privileged information and we only want users with a subscription to that course to have access to the information about the video.

### Creating an Authorization Requirement `IAuthorizationRequirement`

Location: `~/Application/Authorization/MustHaveCourseSubscriptionRequirement.cs`

You can create custom, reusable authorization rules for your MediatR requests by implementing `IAuthorizationRequirement` and `IAuthorizationHandler<TAuthorizationRequirement>`:

```c#
public class MustHaveCourseSubscriptionRequirement : IAuthorizationRequirement
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }

        class MustHaveCourseSubscriptionRequirementHandler : IAuthorizationHandler<MustHaveCourseSubscriptionRequirement>
        {
            private readonly IApplicationDbContext _applicationDbContext;

            public MustHaveCourseSubscriptionRequirementHandler(IApplicationDbContext applicationDbContext)
            {
                _applicationDbContext = applicationDbContext;
            }

            public async Task<AuthorizationResult> Handle(MustHaveCourseSubscriptionRequirement request, CancellationToken cancellationToken)
            {
                var userId = request.UserId;
                var userCourseSubscription = await _applicationDbContext.UserCourseSubscriptions
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.CourseId == request.CourseId, cancellationToken);

                if (userCourseSubscription != null)
                    return AuthorizationResult.Succeed();

                return AuthorizationResult.Fail("You don't have a subscription to this course.");
            }
        }
    }
```
In the preceding listing, you can see this is your standard MediatR Request/Request Handler usage; so you can treat the whole affair as you normally would. It is important to note you must return `AuthorizationResult` You can fail two ways: `AuthorizationResult.Fail()` or `AuthorizationResult.Fail("your message here")` and you can pass using `AuthorizationResult.Succeed()`

### Basic MediatR Request

Location: `~/Application/Courses/Queries/GetCourseVideoDetail/GetCourseVideoDetailQuery.cs`

```c#
public class GetCourseVideoDetailQuery : IRequest<CourseVideoDetailVm>
    {
        public int CourseId { get; set; }
        public int VideoId { get; set; }
        
        class GetCourseVideoDetailQueryHandler : IRequestHandler<GetCourseVideoDetailQuery>
        {
            private readonly IApplicationDbContext _applicationDbContext;

            public GetCourseVideoDetailQueryHandler(IApplicationDbContext applicationDbContext)
            {
                _applicationDbContext = applicationDbContext;
            }

            public async Task<CourseVideoDetailVm> Handle(GetCourseVideoDetailQuery request, CancellationToken cancellationToken)
            {
                var video = await _applicationDbContext.CourseVideos
                    .FirstOrDefaultAsync(x => x.CourseId == request.CourseId && x.VideoId == request.VideoId, cancellationToken);

                return new CourseVideoDetailVm(video);
            }
        }
    }
```

### Creating the `IAuthorizer`

Location: `~/Application/Courses/Queries/GetCourseVideoDetail/GetCourseVideoDetailAuthorizer.cs`

```c#
public class GetCourseVideoDetailAuthorizer : AbstractRequestAuthorizer<GetCourseVideoDetailQuery>
    {
        private readonly ICurrentUserService _currentUserService;

        public GetCourseVideoDetailAuthorizer(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override void BuildPolicy(GetCourseVideoDetailQuery request)
        {
            UseRequirement(new MustHaveCourseSubscriptionRequirement
            {
                CourseId = request.CourseId,
                UserId = _currentUserService.UserId
            });
        }
    }
```
The usage of `AbstractRequestAuthorizer<TRequest>` will usually be preferable; this abstract class does a couple things for us. It takes care of initializing and adding new requirements to the `Requirements` property through the `UseRequirement(IAuthorizationRequirement)`, finally, it still forces the class extending it to implement the `IAuthorizer.BuildPolicy()` method which is very important for passing the needed arguments to the authorization requirement that handles the authorization logic.

## Overriding the Default Unauthorized Behavior

When a requirement is not met (i.e., IsAuthorized is false), the default behavior is to throw an `UnauthorizedException`. You can change this by creating a class which implements the `Invoke` method of the `IUnauthorizedResultHandler` interface:

```c#
public class ExampleUnauthorizedResultHandler : IUnauthorizedResultHandler
    {
        public Task<TResponse> Invoke<TResponse>(AuthorizationResult result)
        {
            return Task.FromResult(default(TResponse));
        }
    }
```

Once you have created your custom `IUnauthorizedResultHandler`, you will need to configure the options during IoC setup:

```c#
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;

public class Startup
{
	//...
	public void ConfigureServices(IServiceCollection services)
	{
		// Use the options overload method to configure your custom `IUnauthorizedResultHandler`
		services.AddMediatorAuthorization(Assembly.GetExecutingAssembly(), 
            cfg => cfg.UseUnauthorizedResultHandlerStrategy(new ExampleUnauthorizedResultHandler));
	}
}
```

The `ExampleUnauthorizedResultHandler` is a very basic example. Some reasons why you may want to override the default unauthorized behavior can include (but not limited to):
* Throwing a different exception type
* Attaching additional behavior such as raising an event.
* If you are using a discriminated union library (e.g., OneOf, FluentResults, ErrorOr) in conjuction with your MediatR requests.


For any requests, bug or comments, please [open an issue][issues] or [submit a
pull request][pulls].

[dotnet-core-cli-tools]: https://docs.microsoft.com/en-us/dotnet/core/tools/
[issues]: https://github.com/AustinDavies/MediatR.Behaviors.Authorization/issues/new
[nuget-cli]: https://docs.microsoft.com/en-us/nuget/tools/nuget-exe-cli-reference
[package-manager-console]: https://docs.microsoft.com/en-us/nuget/tools/package-manager-console
[pulls]: https://github.com/AustinDavies/MediatR.Behaviors.Authorization/pulls
[article]: https://levelup.gitconnected.com/handling-authorization-in-clean-architecture-with-asp-net-core-and-mediatr-6b91eeaa4d15
