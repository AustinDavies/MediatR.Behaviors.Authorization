using System.Collections.Generic;

namespace MediatR.Behaviors.Authorization.Interfaces
{
    public interface IAuthorizer<T>
    {
        IEnumerable<IAuthorizationRequirement> Requirements { get; }
        void BuildPolicy(T instance);
    }
}
