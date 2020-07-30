namespace Runbook.Services.Interfaces
{
    public interface IMailService
    {
        System.Threading.Tasks.Task SendEmail(string toEmail, string subject, string body);
    }
}