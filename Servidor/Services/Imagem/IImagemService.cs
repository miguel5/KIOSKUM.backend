using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Imagem
{
    public interface IImagemService
    {
        ServiceResult<string> ValidaImagem(IFormFile ficheiro);
        Task GuardarImagem(IFormFile ficheiro, string pathAnterior, string pathNova, string webRootPath);

    }
}
