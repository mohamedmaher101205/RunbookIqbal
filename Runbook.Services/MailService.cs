using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Runbook.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Runbook.Services
{
    /// <summary>
    /// This MailService class is to send email for user
    /// </summary>
    public class MailService : IMailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        /// <summary>
        /// This constructor is to inject IConfiguration using constructor dependency injuction
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public MailService(IConfiguration config, ILogger<MailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Send email to the user
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="subscribers"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task SendEmail(string toEmail, string subject, string body, string subscribers)
        {
            try
            {
                List<EmailAddress> emailAddresses = null;
                if (!string.IsNullOrEmpty(toEmail))
                {
                    _logger.LogInformation($"Preparing an EMail to send");
                    var sendGridApiKey = _config["SendGrid:SendGridAPIKey"];
                    var emailClient = new SendGridClient(sendGridApiKey);
                    var message = new SendGridMessage()
                    {
                        From = new EmailAddress(_config["SmtpSettings:SenderEmail"], _config["SmtpSettings:SenderName"]),
                        Subject = subject,
                        HtmlContent = body
                    };
                    var emailaddr = new EmailAddress(toEmail);
                    if (!string.IsNullOrEmpty(subscribers))
                    {
                        emailAddresses  = new List<EmailAddress>();
                        List<string> emailList = subscribers.Split(new char[] { ',' }).ToList();
                        foreach (string email in emailList)
                            emailAddresses.Add(new EmailAddress(email));
                        message.AddTos(emailAddresses);
                    }

                    message.AddTo(new EmailAddress(toEmail, ""));
                    
                    var response = await emailClient.SendEmailAsync(message);
                }
                else
                {
                    _logger.LogError($"Invalid Email Address in SendEmail : {toEmail}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error in SendEmail : {ex}");
                throw ex;
            }
        }
    }
}