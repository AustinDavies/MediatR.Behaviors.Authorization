using System;
using MediatR.Behaviors.Authorization.Exceptions;
using Xunit;

namespace MediatR.Behaviors.Authorization.Tests.Exceptions
{
	public class UnauthorizedExceptionTests
	{
        [Fact(DisplayName = "Should throw 'UnauthorizedException'")]
        public void Should_throw_UnauthorizedException()
          => Assert.ThrowsAsync<UnauthorizedException>(() => throw new UnauthorizedException("Unauthorized"));

        [Fact(DisplayName = "Should throw UnauthorizedException with message")]
        public async void Should_throw_UnauthorizedException_when_message()
        {
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(() => throw new UnauthorizedException("Unauthorized"));
            Assert.Equal("Unauthorized", exception.Message);
        }

    }
}

