using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Entities;
using Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Services.EmailSender
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
        }

        public async Task SendEmail(string email, Email mensagem)
        {
            _logger.LogDebug("A executar [EmailSenderService -> SendEmail]");
            var apikey = _emailSettings.SendGridAPIKey;
            var client = new SendGridClient(apikey);
            var from = new EmailAddress(_emailSettings.MyEmail, _emailSettings.Name);
            var subject = mensagem.Assunto;
            var to = new EmailAddress(email);
            var plainTextContent = "";
            var htmlContent = $"<p>{mensagem.Conteudo}</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
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