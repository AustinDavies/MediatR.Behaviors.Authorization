using System.Threading.Tasks;

namespace MediatR.Behaviors.Authorization.Interfaces
{
    public interface IUnauthorizedResultHandler
    {
        Task<TResponse> Invoke<TResponse>(AuthorizationResult result);
    }
}
