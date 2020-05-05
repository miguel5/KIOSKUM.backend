using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers;
using API.ViewModels.ProdutoDTOs;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace API.Business
{
    public interface IProdutoService
    {
        Task<ServiceResult<int>> RegistarProduto(RegistarProdutoDTO model);
        Task<ServiceResult> EditarProduto(EditarProdutoDTO model);
        IList<ProdutoViewDTO> GetProdutosDesativados();
        ServiceResult<ProdutoViewDTO> GetProduto(int idProduto);
        ServiceResult DesativarProduto(int idProduto);
        ServiceResult AtivarProduto(int idProduto);
    }


    public class ProdutoService : IProdutoService
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IProdutoDAO _produtoDAO;
        private readonly ICategoriaDAO _categoriaDAO;


        public ProdutoService(ILogger<ProdutoService> logger, IOptions<AppSettings> appSettings, IWebHostEnvironment webHostEnviroment, IMapper mapper, IProdutoDAO produtoDAO, ICategoriaDAO categoriaDAO)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _webHostEnvironment = webHostEnviroment;
            _mapper = mapper;
            _produtoDAO = produtoDAO;
            _categoriaDAO = categoriaDAO;
        }

        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [ProdutoService -> ValidaNome]");
            return nome.Length <= 45;
        }

        private bool ValidaPreco(double preco)
        {
            _logger.LogDebug("A executar [ProdutoService -> ValidaPreco]");
            Regex rx = new Regex("^\\d{1,6}(.\\d{1,2})?$");
            return rx.IsMatch(preco.ToString());
        }

        private bool ValidaIngredientes(IList<string> ingredientes)
        {
            _logger.LogDebug("A executar [ProdutoService -> ValidaIngredientes]");
            bool sucesso = true;
            foreach(string ingrediente in ingredientes) if(sucesso)
            {
                sucesso = ingrediente.Length <= 45;
            }
            return sucesso;
        }

        private bool ValidaAlergenios(IList<string> alergenios)
        {
            _logger.LogDebug("A executar [ProdutoService -> ValidaAlergenios]");
            bool sucesso = true;
            foreach (string alergenio in alergenios) if (sucesso)
                {
                    sucesso = alergenio.Length <= 45;
                }
            return sucesso;
        }


        private ServiceResult<string> ValidaImagem(IFormFile ficheiro)
        {
            _logger.LogDebug("A executar [ProdutoService -> ValidaImagem]");

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



        private async Task GuardarImagem(int idProduto, IFormFile ficheiro, string extensaoAnterior, string extensaoNova)
        {
            _logger.LogDebug("A executar [ProdutoService -> GuardarImagem]");
            
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produto", $"{idProduto}.{extensaoNova}");
            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            await ficheiro.CopyToAsync(fileStream);

            if (!extensaoNova.Equals(extensaoAnterior))
            {
                filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produto", $"{idProduto}.{extensaoAnterior}");
                await Task.Factory.StartNew(() => File.Delete(filePath));
            }
            _logger.LogDebug($"Sucesso no upload da imagem do produto com idProduto {idProduto}!");
            
        }


        public async Task<ServiceResult<int>> RegistarProduto(RegistarProdutoDTO model)
        {
            _logger.LogDebug("A executar [ProdutoService -> RegistarProduto]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (model.Ingredientes == null)
            {
                throw new ArgumentNullException("Ingredientes", "Parametro não pode ser nulo");
            }
            if (model.Alergenios == null)
            {
                throw new ArgumentNullException("Alergenios", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            int idProduto = -1;

            if (_produtoDAO.ExisteNomeProduto(model.Nome))
            {
                _logger.LogDebug($"O produto com o nome {model.Nome} já existe no Sistema!");
                Produto produto = _produtoDAO.GetProdutoNome(model.Nome);
                if (_produtoDAO.isAtivo(produto.IdProduto))
                {
                    _logger.LogDebug($"O produto com o nome {model.Nome} já existe no Sistema, com IdProdudo {produto.IdProduto} e encontra-se ativado!");
                    erros.Add((int)ErrosEnumeration.NomeProdutoJaExiste);
                }
                else
                {
                    _logger.LogDebug($"O produto com o nome {model.Nome} já existe no Sistema, com IdProdudo {produto.IdProduto} e encontra-se desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }
            else
            {

                if (!ValidaNome(model.Nome))
                {
                    _logger.LogDebug($"O nome {model.Nome} é inválido!");
                    erros.Add((int)ErrosEnumeration.NomeProdutoInvalido);
                }
                if (!ValidaPreco(model.Preco))
                {
                    _logger.LogDebug($"O preço {model.Preco} é inválido!");
                    erros.Add((int)ErrosEnumeration.PrecoProdutoInvalido);
                }
                if (!_categoriaDAO.ExisteCategoria(model.IdCategoria))
                {
                    _logger.LogDebug($"Não existe nenhuma categoria no sistema com IdCategooria {model.IdCategoria}!");
                    erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
                }
                if (!ValidaIngredientes(model.Ingredientes))
                {
                    _logger.LogDebug($"O nome de um ingrediente é inválido!");
                    erros.Add((int)ErrosEnumeration.IngredientesProdutoInvalidos);
                }
                if (!ValidaAlergenios(model.Alergenios))
                {
                    _logger.LogDebug($"O nome de um alergenio é inválido!");
                    erros.Add((int)ErrosEnumeration.AlergeniosProdutoInvalidos);
                }

                ServiceResult<string> resultado = ValidaImagem(model.File);
                if (!resultado.Sucesso)
                {
                    erros.Concat(resultado.Erros.Erros);
                }

                if (!erros.Any())
                {
                    Produto produto = _mapper.Map<Produto>(model);
                    produto.ExtensaoImagem = resultado.Resultado;
                    idProduto = _produtoDAO.RegistarProduto(produto);
                    await GuardarImagem(idProduto, model.File, "", resultado.Resultado);
                }
            }
            return new ServiceResult<int> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = idProduto};
        }

        public async Task<ServiceResult> EditarProduto(EditarProdutoDTO model)
        {
            _logger.LogDebug("A executar [ProdutoService -> EditarProduto]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (model.Ingredientes == null)
            {
                throw new ArgumentNullException("Ingredientes", "Parametro não pode ser nulo");
            }
            if (model.Alergenios == null)
            {
                throw new ArgumentNullException("Alergenios", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(model.IdProduto);

            if (produto == null)
            {
                _logger.LogDebug($"Não existe nenhum produto com IdProduto {model.IdProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.isAtivo(model.IdProduto))
                {
                    if (!produto.Nome.Equals(model.Nome) && produto.Nome.Equals(model.Nome))
                    {

                        _logger.LogDebug($"O nome {model.Nome} já existe no sistema!");
                        erros.Add((int)ErrosEnumeration.NomeProdutoJaExiste);
                    }
                    else
                    {
                        if (!ValidaNome(model.Nome))
                        {
                            _logger.LogDebug($"O nome {model.Nome} é inválido!");
                            erros.Add((int)ErrosEnumeration.NomeProdutoInvalido);
                        }
                        if (!ValidaPreco(model.Preco))
                        {
                            _logger.LogDebug($"O preço {model.Preco} é inválido!");
                            erros.Add((int)ErrosEnumeration.PrecoProdutoInvalido);
                        }
                        if (!_categoriaDAO.ExisteCategoria(model.IdCategoria))
                        {
                            _logger.LogDebug($"Não existe nenhuma categoria no sistema com IdCategooria {model.IdCategoria}!");
                            erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
                        }
                        if (!ValidaIngredientes(model.Ingredientes))
                        {
                            _logger.LogDebug($"O nome de um ingrediente é inválido!");
                            erros.Add((int)ErrosEnumeration.IngredientesProdutoInvalidos);
                        }
                        if (!ValidaAlergenios(model.Alergenios))
                        {
                            _logger.LogDebug($"O nome de um alergenio é inválido!");
                            erros.Add((int)ErrosEnumeration.AlergeniosProdutoInvalidos);
                        }

                        ServiceResult<string> result = ValidaImagem(model.File);
                        if (!result.Sucesso)
                        {
                            erros.Concat(result.Erros.Erros);
                        }


                        if (!erros.Any())
                        {
                            Produto novoProduto = _mapper.Map<Produto>(model);
                            novoProduto.ExtensaoImagem = result.Resultado;
                            _produtoDAO.EditarProduto(novoProduto);
                            await GuardarImagem(model.IdProduto, model.File, produto.ExtensaoImagem, novoProduto.ExtensaoImagem);
                        }
                    }
                }
                else
                {
                    _logger.LogDebug($"O produto com idProduto {model.IdProduto} encontra-se desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public IList<ProdutoViewDTO> GetProdutosDesativados()
        {
            _logger.LogDebug("A executar [ProdutoService -> GetProdutosDesativados]");
            IList<ProdutoViewDTO> produtosViewDTO = null;

            IList<Produto> produtos = _produtoDAO.GetProdutosDesativados();
            if (produtos != null)
            {
                string pathImagem = Path.Combine(_appSettings.ServerUrl, "Images", "Produto");
                foreach (Produto produto in produtos)
                {
                    ProdutoViewDTO produtoViewDTO = _mapper.Map<ProdutoViewDTO>(produto);
                    produtoViewDTO.Url = new Uri(Path.Combine(pathImagem, $"{produto.IdProduto}.{produto.ExtensaoImagem}"));
                    produtosViewDTO.Add(produtoViewDTO);
                }
            }
            else
            {
                produtosViewDTO = new List<ProdutoViewDTO>();
            }

            return produtosViewDTO;
        }



        public ServiceResult<ProdutoViewDTO> GetProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoService -> GetProduto]");
            IList<int> erros = new List<int>();
            ProdutoViewDTO produtoViewDTO = null;

            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                _logger.LogDebug($"Não existe nenhum produto com IdProduto {idProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.isAtivo(idProduto))
                {
                    produtoViewDTO = _mapper.Map<ProdutoViewDTO>(produto);
                    produtoViewDTO.Url = new Uri(Path.Combine(_appSettings.ServerUrl, "Images", "Produto", $"{produto.IdProduto}.{produto.ExtensaoImagem}"));
                }
                else
                {
                    _logger.LogDebug($"O produto com idProduto {idProduto} encontra-se desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }

            return new ServiceResult<ProdutoViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = produtoViewDTO };
        }


        public ServiceResult DesativarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoService -> DesativarProduto]");

            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                _logger.LogDebug($"Não existe nenhum produto com IdProduto {idProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.isAtivo(idProduto))
                {
                    _produtoDAO.DesativarProduto(idProduto);
                }
                else
                {
                    _logger.LogDebug($"O produto com idProduto {idProduto} já se encontra desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult AtivarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoService -> AtivarProduto]");
            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                _logger.LogDebug($"Não existe nenhum produto com IdProduto {idProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.isAtivo(idProduto))
                {
                    _logger.LogDebug($"O produto com idProduto {idProduto} já se encontra ativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoAtivado);
                }
                else
                {
                    _produtoDAO.AtivarProduto(idProduto);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }
    }
}
 