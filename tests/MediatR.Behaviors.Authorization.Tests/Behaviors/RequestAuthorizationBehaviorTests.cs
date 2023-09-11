using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;
using System.Threading;
using MediatR.Behaviors.Authorization.Exceptions;

namespace MediatR.Behaviors.Authorization.Tests.Behaviors
{
    public class RequestAuthorizationBehaviorTests
    {

        #region Class mock
        public class RequestClassMock1 : IRequest<ResponseClassMock1>
        {
            public int Id { get; set; }
        }

        public class ResponseClassMock1
        {
            public string MESSAGE { get; } = "SUCCESS";
        }
          

        public class RequestClassMock1Handler : IRequestHandler<RequestClassMock1, ResponseClassMock1>
        {
            async Task<ResponseClassMock1> IRequestHandler<RequestClassMock1, ResponseClassMock1>.Handle(RequestClassMock1 request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new ResponseClassMock1());
            }
        }
        #endregion


        [Fact(DisplayName = "Should execute handler successfully when there is no implementation of 'Requirement' or 'IAuthorizationRequirement' and not registry extensions")]
        public async Task Should_execute_handler_success_when_there_is_no_implementation_of_Requirement_or_IAuthorizationRequirement_and_not_registry_extensions()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly));
           
            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<ISender>();

            var response = await mediator.Send(new RequestClassMock1());

            Assert.Equal("SUCCESS", response.MESSAGE);
        }


        [Fact(DisplayName = "Should execute handler successfully when there is no implementation of 'Requirement' or 'IAuthorizationRequirement'")]
        public async Task Should_execute_handler_success_when_there_is_no_implementation_of_Requirement_or_IAuthorizationRequirement()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly));

            services.AddMediatorAuthorization(typeof(RequestAuthorizationBehaviorTests).Assembly);
            services.AddAuthorizersFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<ISender>();

            var response = await mediator.Send(new RequestClassMock1());

            Assert.Equal("SUCCESS", response.MESSAGE);
        }



        #region Class mock
        public class RequestClassMock2 : IRequest<ResponseClassMock2>
        {
            public int Id { get; set; }
        }

        public class ResponseClassMock2
        {
            public string MESSAGE { get; } = "SUCCESS";
        }

        public class RequestClassMock2Authorizer : AbstractRequestAuthorizer<RequestClassMock2>
        {

            public override void BuildPolicy(RequestClassMock2 request)
            {
                UseRequirement(new RequestClassMock2Requirement
                {
                    Id = request.Id
                });
            }
        }


        public class RequestClassMock2Requirement : IAuthorizationRequirement
        {
            public int Id { get; set; }

            class RequestClassMock2RequirementHandler : IAuthorizationHandler<RequestClassMock2Requirement>
            {

                public async Task<AuthorizationResult> Handle(RequestClassMock2Requirement request, CancellationToken cancellationToken)
                {
                    if (request.Id % 2 == 0) return AuthorizationResult.Succeed();

                    return AuthorizationResult.Fail();
                }
            }
        }

        public class SingHandler : IRequestHandler<RequestClassMock2, ResponseClassMock2>
        {
            async Task<ResponseClassMock2> IRequestHandler<RequestClassMock2, ResponseClassMock2>.Handle(RequestClassMock2 request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new ResponseClassMock2());
            }
        }
        #endregion

        [Fact(DisplayName = "Should return message 'SUCCESS")]
        public async Task Should_return_MESSAGE_SUCCESS()
        { 
            IServiceCollection services = new ServiceCollection();
             
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly));

            services.AddMediatorAuthorization(typeof(RequestAuthorizationBehaviorTests).Assembly);
            services.AddAuthorizersFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<ISender>();

            var response = await mediator.Send(new RequestClassMock2 { Id = 10 });

            Assert.Equal("SUCCESS", response.MESSAGE);

        }

        [Fact(DisplayName = "Should throw UnauthorizedException when AuthorizationResult is Fail")]
        public async Task Should_throw_UnauthorizedException_when_AuthorizationResult_is_Fail()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly));

            services.AddMediatorAuthorization(typeof(RequestAuthorizationBehaviorTests).Assembly);
            services.AddAuthorizersFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<ISender>();

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => mediator.Send(new RequestClassMock2() { Id = 13 })
            ); 
        }


        #region Class mock
        public class RequestClassMock3 : IRequest<ResponseClassMock3>
        {
            public int Id { get; set; }
        }

        public class ResponseClassMock3
        {
            public string MESSAGE { get; } = "SUCCESS";
        }

        public class RequestClassMock3Authorizer : AbstractRequestAuthorizer<RequestClassMock3>
        {

            public override void BuildPolicy(RequestClassMock3 request)
            {
                UseRequirement(new RequirementMock3
                {
                    Id = request.Id
                });
            }
        }


        public class RequirementMock3 : IAuthorizationRequirement
        {
            public int Id { get; set; }
             
        }

        public class RequestClassMock3Handler : IRequestHandler<RequestClassMock3, ResponseClassMock3>
        {
            async Task<ResponseClassMock3> IRequestHandler<RequestClassMock3, ResponseClassMock3>.Handle(RequestClassMock3 request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new ResponseClassMock3());
            }
        }
        #endregion

        [Fact(DisplayName = "Should throw InvalidOperationException when not find an AuthorizationHandler implementation for requirement")]
        public async Task Should_throw_InvalidOperationException_when_not_find_an_AuthorizationHandler_implementation_for_requirement()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly));

            services.AddMediatorAuthorization(typeof(RequestAuthorizationBehaviorTests).Assembly);
            services.AddAuthorizersFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<ISender>();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => mediator.Send(new RequestClassMock3())
            );
        }




        #region Class mock
        public class RequestClassMock4 : IRequest<ResponseClassMock4>
        {
            public int Id { get; set; }
        }

        public class ResponseClassMock4
        {
            public string MESSAGE { get; } = "SUCCESS";
        }

        public class RequestClassMock4Authorizer : AbstractRequestAuthorizer<RequestClassMock4>
        {

            public override void BuildPolicy(RequestClassMock4 request)
            {
                UseRequirement(new RequestClassMock4Requirement
                {
                    Id = request.Id
                });
            }
        }


        public class RequestClassMock4Requirement : IAuthorizationRequirement
        {
            public int Id { get; set; }
             
        }

        public class RequestClassMock4Handler : IRequestHandler<RequestClassMock4, ResponseClassMock4>
        {
            async Task<ResponseClassMock4> IRequestHandler<RequestClassMock4, ResponseClassMock4>.Handle(RequestClassMock4 request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new ResponseClassMock4());
            }
        }

        public class RequestClassMock4RequirementHandler2 : IAuthorizationHandler<RequestClassMock4Requirement>
        {

            public async Task<AuthorizationResult> Handle(RequestClassMock4Requirement request, CancellationToken cancellationToken)
            {
                if (request.Id % 2 == 0) return AuthorizationResult.Succeed();

                return AuthorizationResult.Fail();
            }
        }

        public class RequestClassMock4RequirementHandler3 : IAuthorizationHandler<RequestClassMock4Requirement>
        {

            public async Task<AuthorizationResult> Handle(RequestClassMock4Requirement request, CancellationToken cancellationToken)
            {
                if (request.Id % 2 == 0) return AuthorizationResult.Succeed();

                return AuthorizationResult.Fail();
            }
        }
        #endregion

        [Fact(DisplayName = "Should throw InvalidOperationException when Requirement has multiple authorization handlers implementations")]
        public async Task Should_throw_InvalidOperationException_when_Requirement_has_multiple_AuthorizationHandle_implementations()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly));

            services.AddMediatorAuthorization(typeof(RequestAuthorizationBehaviorTests).Assembly);
            services.AddAuthorizersFromAssembly(typeof(RequestAuthorizationBehaviorTests).Assembly);

            var provider = services.BuildServiceProvider();

            var mediator = provider.GetRequiredService<ISender>();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => mediator.Send(new RequestClassMock4())
            );
        }

    }
}

