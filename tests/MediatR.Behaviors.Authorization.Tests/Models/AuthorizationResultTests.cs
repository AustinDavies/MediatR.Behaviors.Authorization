using Xunit;

namespace MediatR.Behaviors.Authorization.Tests.Models
{
    public class AuthorizationResultTests
	{

        [Fact(DisplayName = "Should return FailureMessage null when call 'Succeed()'.")]
        public void Should_return_FailureMessage_null_when_call_Succeed()
            => Assert.Null(AuthorizationResult.Succeed().FailureMessage);

        [Fact(DisplayName = "Should return IsAuthorized true when call function 'Succeed'")]
        public void Should_return_IsAuthorized_true_when_call_function_Succeed()
            => Assert.True(AuthorizationResult.Succeed().IsAuthorized);

        [Fact(DisplayName = "Should return IsAuthorized false when call function 'Fail'")]
        public void Should_return_IsAuthorized_false_when_call_function_Fail()
            => Assert.False(AuthorizationResult.Fail().IsAuthorized);

        [Fact(DisplayName = "Should return FailureMessage null when call function 'Fail()'.")]
        public void Should_return_FailureMessage_null()
            => Assert.Null(AuthorizationResult.Fail().FailureMessage);

        [Fact]
        public void Should_return_FailureMessage()
            => Assert.Equal("Fail message", AuthorizationResult.Fail("Fail message").FailureMessage);

    }
}

