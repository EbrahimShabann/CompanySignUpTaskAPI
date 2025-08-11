
using System.Net;
using System.Net.Mail;

namespace CompanySignUpTask.Service_Layer.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            //config smtp client 
            var smtpClient = new SmtpClient()
            {
                Host= config["EmailSettings:SmtpServer"],
                Port = int.Parse(config["EmailSettings:Port"]),
                Credentials=new NetworkCredential
                (
                    config["EmailSettings:SenderEmail"],
                    config["EmailSettings:Password"]
                ),
                EnableSsl=true,
                UseDefaultCredentials = false 
            };

            //create mail message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(config["EmailSettings:SenderEmail"], config["EmailSettings:SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
