using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    public interface IAuthService
    {
        AuthRequest AuthenticateUser(User user);

        string RegisterUser(User user);

        AuthRequest OpenIdAuthenticateUser(User user);

        bool checkExistingUser  (User user);

        string OTPGenrate();

        string ResetPassword(User user);

    }
}