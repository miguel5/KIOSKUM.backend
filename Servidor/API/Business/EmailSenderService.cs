using System;
using System.Net.Mail;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Business
{
    public interface IEmailSenderService : IDisposable
    {
        Task SendEmail(string email, Email mensagem);
    }

    public class EmailSenderService : IEmailSenderService
    {

        private readonly ILogger<EmailSenderService> _logger;
        private readonly EmailSettings _emailSettings;
        private readonly SmtpClient _smtp;


        public EmailSenderService(ILogger<EmailSenderService> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _emailSettings = appSettings.Value.EmailSettings;
            _smtp = new SmtpClient
            {
                Host = _emailSettings.ServerAddressSMTP, 
                Credentials = new System.Net.NetworkCredential
                 (_emailSettings.MyEmail, _emailSettings.MyPassword),
                Port = _emailSettings.Port,
                EnableSsl = true
            };
        }

        public async Task SendEmail(string email, Email mensagem)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress(_emailSettings.MyEmail);
            mail.Subject = mensagem.Assunto;
            mail.Body = mensagem.Conteudo;
            mail.IsBodyHtml = true;
            await _smtp.SendMailAsync(mail);
        }

        public void Dispose()
        {
            _smtp.Dispose();
        }
    }
}