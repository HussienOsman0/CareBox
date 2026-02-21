using CareBox.BLL.Services.EmailServices.Interfaces;
using CareBox.BLL.Services.EmailServices.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string mailTo, string subject, string body)
        {
            var email = new MimeMessage();

            // المرسل
            email.Sender = MailboxAddress.Parse(_mailSettings.Email);
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

            // المستقبل
            email.To.Add(MailboxAddress.Parse(mailTo));

            // الموضوع
            email.Subject = subject;

            // المحتوى (HTML)
            var builder = new BodyBuilder();
            builder.HtmlBody = body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                // الاتصال بالسيرفر (Gmail, Outlook, etc.)
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

                // تسجيل الدخول
                await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);

                // الإرسال
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
