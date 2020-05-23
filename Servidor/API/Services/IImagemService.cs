using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Http;

namespace API.Services
{
    public interface IImagemService
    {
        ServiceResult<string> ValidaImagem(IFormFile ficheiro);
        Task GuardarImagem(IFormFile ficheiro, string pathAnterior, string pathNova);

    }
}
