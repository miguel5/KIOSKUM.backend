using System;
using System.Threading.Tasks;
using Entities;

namespace Services.EmailSender
{
    public interface IEmailSenderService : IDisposable
    {
        Task SendEmail(string email, Email mensagem);
    }
}