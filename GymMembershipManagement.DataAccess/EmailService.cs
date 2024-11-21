using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace GymMembershipManagement.DataAccess
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;

        public EmailService(IConfiguration configuration)
        {
            var smtpSettings = configuration.GetSection("MailSettings:MailTrap");
            _smtpHost = smtpSettings["Host"];
            _smtpPort = int.Parse(smtpSettings["Port"]);
            _enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true"); 
            _smtpUser = smtpSettings["Username"];
            _smtpPassword = smtpSettings["Password"];
        }


        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = _enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@urbangym.com", "Urban GYM"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
