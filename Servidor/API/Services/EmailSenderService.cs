using System.Net.Mail;
using System.Threading.Tasks;
using API.Models;

namespace API.Business
{
    public interface IEmailSenderService
    {
        Task SendEmail(string email, Email mensagem);
    }

    public class EmailSenderService : IEmailSenderService
    {
        private readonly string myEmail = "espeta_espeta@portugalmail.pt";
        private readonly string myPassword = "meteantonio";
        private readonly string serverAdressSMTP = "smtp.portugalmail.pt";
        private readonly SmtpClient smtp;

        public EmailSenderService()
        {
            smtp = new SmtpClient
            {
                Host = serverAdressSMTP, //Or Your SMTP Server Address
                Credentials = new System.Net.NetworkCredential
                 (myEmail, myPassword), // ***use valid credentials***
                Port = 587,
                EnableSsl = false
            };
        }


        
        public async Task SendEmail(string email, Email mensagem)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress(myEmail);
            mail.Subject = mensagem.Assunto;
            mail.Body = mensagem.Conteudo;
            mail.IsBodyHtml = true;
            
            await smtp.SendMailAsync(mail);
        }
        
    }
}
