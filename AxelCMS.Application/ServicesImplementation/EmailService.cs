using AxelCMS.Application.Interfaces.Services;
using AxelCMS.Domain.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AxelCMS.Application.ServicesImplementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendHtmlEmailAsync(MailRequest mailRequest)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_emailSettings.Email)
            };
            message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            message.Subject = mailRequest.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = mailRequest.Body
            };

            byte[] fileBytes1;
            byte[] fileBytes2;

            if (System.IO.File.Exists("EmailAttachments/freedom.jpeg") && System.IO.File.Exists("EmailAttachments/lorem.jpeg"))
            {
                using (var ms1 = new MemoryStream())
                using (var file1 = new FileStream("EmailAttachments/freedom.jpeg", FileMode.Open, FileAccess.Read))
                {
                    file1.CopyTo(ms1);
                    fileBytes1 = ms1.ToArray();
                    builder.Attachments.Add("attachment1.jpeg", fileBytes1, ContentType.Parse("application/octet-stream"));
                }

                using (var ms2 = new MemoryStream())
                using (var file2 = new FileStream("EmailAttachments/lorem.jpeg", FileMode.Open, FileAccess.Read))
                {
                    file2.CopyTo(ms2);
                    fileBytes2 = ms2.ToArray();
                    builder.Attachments.Add("attachment2.jpeg", fileBytes2, ContentType.Parse("application/octet-stream"));
                }
            }
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                    await client.SendAsync(message);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Email sending failed", ex);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public async Task SendEmailconfirmationAsync(MailRequest mailRequest, string emailConfirmationToken)
        {
            var message = new MimeMessage();
            message.Sender = MailboxAddress.Parse(_emailSettings.Email);
            message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            message.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
                    await client.SendAsync(message);
                }
                catch(Exception ex)
                {
                    throw new InvalidOperationException("Email sending failed", ex.InnerException);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
