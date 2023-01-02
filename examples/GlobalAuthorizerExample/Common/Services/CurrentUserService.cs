namespace GlobalAuthorizerExample.Common.Services
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        bool IsAuthenticated { get; }
    }
    public class ExampleCurrentUserService : ICurrentUserService
    {
        public int UserId { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public void SetAuthenticatedUser(int userId)
        {
            UserId = userId;
            IsAuthenticated = true;
        }
    }
}
