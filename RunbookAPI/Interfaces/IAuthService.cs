namespace RunbookAPI.Models
{
    public interface IAuthService
    {
        AuthRequest AuthenticateUser(User user);

        bool RegisterUser(User user);

        AuthRequest OpenIdAuthenticateUser(User user);

    }
}