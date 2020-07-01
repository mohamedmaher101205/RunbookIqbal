using System.Security.AccessControl;
namespace RunbookAPI.Models
{
    public interface IMailService
    {
        System.Threading.Tasks.Task SendEmail(string toEmail,string subject,string body);
    }
}