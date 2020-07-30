using Runbook.Models;

namespace Runbook.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        AuthRequest GenerateToken(User user);
    }
}