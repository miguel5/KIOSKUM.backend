using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Business
{
    public interface IImagemService
    {
        ServiceResult<string> ValidaImagem(IFormFile ficheiro);
        Task GuardarImagem(IFormFile ficheiro, string pathAnterior, string pathNova);

    }

    public class ImagemService : IImagemService
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImagemService(ILogger<ImagemService> logger, IWebHostEnvironment webHostEnviroment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnviroment;
        }



        public ServiceResult<string> ValidaImagem(IFormFile ficheiro)
        {
            _logger.LogDebug("A executar [ImagemService -> ValidaImagem]");

            IList<int> erros = new List<int>();
            string extensao = null;

            string fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(ficheiro.ContentDisposition).FileName);
            if (fileExtension.Contains('.'))
            {
                fileExtension = fileExtension.Trim('"').Trim('.');
                if (Enum.IsDefined(typeof(ExtensoesValidasEnumeration), fileExtension))
                {
                    if (ficheiro.Length > 0)
                    {
                        extensao = fileExtension;
                    }
                    else
                    {
                        _logger.LogDebug("O ficheiro não possuí conteudo!");
                        erros.Add((int)ErrosEnumeration.ImagemVazia);
                    }
                }
                else
                {
                    _logger.LogDebug($"O formato {fileExtension}, foi rejeitado pelo sistema!");
                    erros.Add((int)ErrosEnumeration.FormatoImagemInvalido);
                }
            }
            else
            {
                _logger.LogDebug("O ficheiro não possuí extensão!");
                erros.Add((int)ErrosEnumeration.FormatoImagemInvalido);
            }
            return new ServiceResult<string> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = extensao };
        }



        public async Task GuardarImagem(IFormFile ficheiro, string pathAnterior, string pathNova)
        {
            _logger.LogDebug("A executar [ImagemService -> GuardarImagem]");

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, pathNova);
            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            var copyTask = ficheiro.CopyToAsync(fileStream);
            Task taskDelete = Task.CompletedTask;

            if (!pathNova.Equals(pathAnterior))
            {
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, pathAnterior);
                taskDelete = Task.Factory.StartNew(() => File.Delete(filePath));
            }
            await Task.WhenAll(new[] { copyTask, taskDelete });

            _logger.LogDebug($"Sucesso no upload da imagem para a path {pathNova}!");
        }

    }
}
