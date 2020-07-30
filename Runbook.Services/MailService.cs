using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Runbook.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;

namespace Runbook.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public MailService(IConfiguration config, ILogger<MailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task SendEmail(string toEmail, string subject, string body)
        {
            try
            {
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