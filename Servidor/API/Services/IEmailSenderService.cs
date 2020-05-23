using System;
using System.Threading.Tasks;
using API.Entities;

namespace API.Services
{
    public interface IEmailSenderService : IDisposable
    {
        Task SendEmail(string email, Email mensagem);
    }
}
