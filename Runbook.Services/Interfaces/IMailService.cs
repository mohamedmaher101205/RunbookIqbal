using System.Collections.Generic;

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
        /// <param name="toEmailLst"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task SendEmail(List<string> toEmailLst, string subject, string body);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        System.Threading.Tasks.Task SendEmail(string toEmail, string subject, string body);

    }
}