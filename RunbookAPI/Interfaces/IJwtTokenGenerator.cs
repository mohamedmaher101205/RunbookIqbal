namespace RunbookAPI.Models
{
    public interface IJwtTokenGenerator
    {
        AuthRequest GenerateToken(User user);
    }
}