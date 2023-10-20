using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Behaviors.Authorization.Tests.Interfaces
{
    public class AuthorizationHandlerTests
	{

        public class ClassMockRequirement : IAuthorizationRequirement
        {
            public int Id { get; set; }

        }

        public class AuthorizationHandler : IAuthorizationHandler<ClassMockRequirement>
        {
            public async Task<AuthorizationResult> Handle(ClassMockRequirement requirement, CancellationToken cancellationToken = default)
            {
                if (requirement.Id % 2 == 0) return AuthorizationResult.Succeed();

                return AuthorizationResult.Fail("Unauthorized");
            }
        }


        [Fact(DisplayName = "Authorization handler should return AuthorizationResult")]
        public async void Should_return_AuthorizationResult()
		{

            IAuthorizationHandler<ClassMockRequirement> authorizationHandler = new AuthorizationHandler();

            var result = await authorizationHandler.Handle(new ClassMockRequirement());

            Assert.IsType<AuthorizationResult>(result);
        }

        [Fact(DisplayName = "Authorization handler should return IsAuthorized")]
        public async void Should_return_IsAuthorized()
        {
            IAuthorizationHandler<ClassMockRequirement> authorizationHandler = new AuthorizationHandler();
            var result = await authorizationHandler.Handle(new ClassMockRequirement() { Id = 10 });
            Assert.True(result.IsAuthorized);
        }

        [Fact(DisplayName = "When authorized should not return a FailureMessage")]
        public async void When_authorized_should_not_return_a_FailureMessage()
        {
            IAuthorizationHandler<ClassMockRequirement> authorizationHandler = new AuthorizationHandler();
            var result = await authorizationHandler.Handle(new ClassMockRequirement() { Id = 10 });
            Assert.True(result.IsAuthorized && string.IsNullOrWhiteSpace(result.FailureMessage));
        }

        [Fact(DisplayName = "Should not return authorized")]
        public async void Should_not_return_authorized()
        {
            IAuthorizationHandler<ClassMockRequirement> authorizationHandler = new AuthorizationHandler();
            var result = await authorizationHandler.Handle(new ClassMockRequirement() { Id = 13 });
            Assert.False(result.IsAuthorized);
        }

        [Fact(DisplayName = "When is not authorized can return FailureMessage")]
        public async void When_is_not_authorized_can_return_FailureMessage()
        {
            IAuthorizationHandler<ClassMockRequirement> authorizationHandler = new AuthorizationHandler();
            var result = await authorizationHandler.Handle(new ClassMockRequirement() { Id = 13 });
            Assert.True(!result.IsAuthorized && !string.IsNullOrWhiteSpace(result.FailureMessage));
        }
    }
}

