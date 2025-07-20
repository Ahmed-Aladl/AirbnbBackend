using System.Net;
using System.Net.Mail;

namespace Airbnb.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public GmailEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = _config["EmailSettings:Email"];
            var password = _config["EmailSettings:AppPassword"];

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            };

            var mail = new MailMessage(email, to, subject, body);
            await smtp.SendMailAsync(mail);
        }
    }
}
