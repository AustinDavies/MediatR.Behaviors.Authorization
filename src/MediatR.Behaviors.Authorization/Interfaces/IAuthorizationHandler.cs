using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Behaviors.Authorization
{
    public interface IAuthorizationHandler<TRequirement>
        where TRequirement : IAuthorizationRequirement
    {
        Task<AuthorizationResult> Handle(TRequirement requirement, CancellationToken cancellationToken = default);
    }
}