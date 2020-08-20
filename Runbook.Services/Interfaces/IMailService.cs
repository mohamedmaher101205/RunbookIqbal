namespace Runbook.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMailService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="subscribers"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task SendEmail(string toEmail, string subject, string body, string subscribers);
    }
}