using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Entities;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Services.Imagem
{
    public class ImagemService : IImagemService
    {
        private readonly ILogger _logger;

        public ImagemService(ILogger<ImagemService> logger)
        {
            _logger = logger;
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

        public async Task GuardarImagem(IFormFile ficheiro, string pathAnterior, string pathNova, string webRootPath)
        {
            _logger.LogDebug("A executar [ImagemService -> GuardarImagem]");

            string filePath = Path.Combine(webRootPath, pathNova);
            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            var copyTask = ficheiro.CopyToAsync(fileStream);
            Task taskDelete = Task.CompletedTask;

            if (!pathNova.Equals(pathAnterior) && !(pathAnterior is null))
            {
                filePath = Path.Combine(webRootPath, pathAnterior);
                taskDelete = Task.Factory.StartNew(() => File.Delete(filePath));
            }
            await Task.WhenAll(new[] { copyTask, taskDelete });

            _logger.LogDebug($"Sucesso no upload da imagem para a path {pathNova}!");
        }

    }
}