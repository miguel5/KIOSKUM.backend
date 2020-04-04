using System;
using System.Net.Mail;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using Microsoft.Extensions.Options;

namespace API.Business
{
    public interface IEmailSenderService
    {
        Task SendEmail(string email, Email mensagem);
    }

    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpClient _smtp;
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptions<AppSettings> appSettings)
        {
            _emailSettings = appSettings.Value.EmailSettings;
            _smtp = new SmtpClient
            {
                Host = _emailSettings.ServerAddressSMTP, //Or Your SMTP Server Address
                Credentials = new System.Net.NetworkCredential
                 (_emailSettings.MyEmail, _emailSettings.MyPassword), // ***use valid credentials***
                Port = _emailSettings.Port,
                EnableSsl = false
            };
        }



        public async Task SendEmail(string email, Email mensagem)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(email);
                mail.From = new MailAddress(_emailSettings.MyEmail);
                mail.Subject = mensagem.Assunto;
                mail.Body = mensagem.Conteudo;
                mail.IsBodyHtml = true;
                await _smtp.SendMailAsync(mail);
            }
            catch (SmtpException)
            {
                Console.WriteLine("Deu merda");
            }
        }

    }
}