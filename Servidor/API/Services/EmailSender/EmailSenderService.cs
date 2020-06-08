using System;
using System.Net.Mail;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Services.EmailSender
{
    public class EmailSenderService : IEmailSenderService, IDisposable
    {

        private readonly ILogger<EmailSenderService> _logger;
        private readonly EmailSettings _emailSettings;
        private SmtpClient _smtp;


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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EmailSenderService() => Dispose(false);

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_smtp != null) { _smtp.Dispose(); _smtp = null; }
            }
        }
    }
}