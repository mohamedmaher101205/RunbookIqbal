using System;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using RunbookAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RunbookAPI.Services
{
    public class MailService : IMailService
    {
        private IConfiguration _config;
        private ILogger _logger;

        public MailService(IConfiguration config,ILogger<MailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task SendEmail(string toEmail,string subject,string body)
        {
            try
            {
                if(!string.IsNullOrEmpty(toEmail))
                {
                    _logger.LogInformation($"Preparing an EMail to send");
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(_config["SmtpSettings:SenderName"],_config["SmtpSettings:SenderEmail"]));
                    message.To.Add(new MailboxAddress("",toEmail));
                    message.Subject = subject;
                    message.Body = new TextPart("html"){
                        Text = body
                    };

                    using (var client = new SmtpClient())
                    {
                        _logger.LogInformation($"Sending EMail to - {toEmail}");
                        client.ServerCertificateValidationCallback = (s,c,h,e) => true;
                        await client.ConnectAsync(_config["SmtpSettings:Server"],int.Parse(_config["SmtpSettings:Port"]),false);
                        await client.AuthenticateAsync(_config["SmtpSettings:SenderEmail"],_config["SmtpSettings:Password"]);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                        _logger.LogInformation($"EMail sent to - {toEmail} successfully");
                    }
                }
                else{
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