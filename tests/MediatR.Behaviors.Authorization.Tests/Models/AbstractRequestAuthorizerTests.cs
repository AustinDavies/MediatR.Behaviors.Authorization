using System;
using System.Linq;
using Xunit;

namespace MediatR.Behaviors.Authorization.Tests.Models
{

    public class AbstractRequestAuthorizerTests
    {
        public class GetCourseVideoDetailQuery : IRequest<object>
        {
            public int CourseId { get; set; } 
        }

        public class MustHaveCourseSubscriptionRequirement : IAuthorizationRequirement
        {
            public int CourseId { get; set; } 
        }


        private class RequestAuthorizerImplementation : AbstractRequestAuthorizer<GetCourseVideoDetailQuery>
        {
            public override void BuildPolicy(GetCourseVideoDetailQuery request)
            {
                UseRequirement(new MustHaveCourseSubscriptionRequirement
                {
                    CourseId = request.CourseId
                });
            }
        }



        [Fact(DisplayName = "Should create 'RequestAuthorizerImplementation'")]
        public void Should_create_RequestAuthorizerImplementation()
          => Assert.NotNull(new RequestAuthorizerImplementation());

        [Fact(DisplayName = "Should create 'RequestAuthorizerImplementation' with Requirements empty")]
        public void Should_create_RequestAuthorizerImplementation_with_Requirements_empty()
            => Assert.Empty(new RequestAuthorizerImplementation().Requirements);

        [Fact(DisplayName = "Should add Requirements when build Policy")]
        public void Should_add_Requirements_when_BuildPolicy()
        {
            var requestAuthorize = new RequestAuthorizerImplementation();
            requestAuthorize.BuildPolicy(new GetCourseVideoDetailQuery() { CourseId = 2});
            Assert.Single(requestAuthorize.Requirements);
        }

        [Fact(DisplayName = "Should clear requirement list")]
        public void Should_clear_requirement_list()
        {
            var requestAuthorize = new RequestAuthorizerImplementation();
            requestAuthorize.BuildPolicy(new GetCourseVideoDetailQuery() { CourseId = 2 });
            requestAuthorize.ClearRequirements();
            Assert.Empty(requestAuthorize.Requirements);
        }
         
    }
}

