using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using API.Data;
using API.Entities;
using API.Helpers;
using API.ViewModels;
using API.ViewModels.ProdutoDTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Business
{
    public interface IProdutoService
    {
        ServiceResult<Tuple<string, string>> RegistarProduto(RegistarProdutoDTO model, string extensao);
        ServiceResult<Tuple<string, string>> EditarProduto(EditarProdutoDTO model, string extensao);
        IList<ProdutoViewDTO> GetProdutosDesativados();
        ServiceResult<ProdutoViewDTO> GetProduto(int idProduto);
        ServiceResult DesativarProduto(int idProduto);
        ServiceResult AtivarProduto(int idProduto);
    }


    public class ProdutoService : IProdutoService
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IProdutoDAO _produtoDAO;
        private readonly ICategoriaDAO _categoriaDAO;


        public ProdutoService(ILogger<ProdutoService> logger, IOptions<AppSettings> appSettings, IMapper mapper, IProdutoDAO produtoDAO, ICategoriaDAO categoriaDAO)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
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


        public ServiceResult<Tuple<string, string>> RegistarProduto(RegistarProdutoDTO model, string extensao)
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
            if(extensao == null)
            {
                throw new ArgumentNullException("Extensao", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            Tuple<string, string> paths = null;

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

                if (!erros.Any())
                {
                    Produto produto = _mapper.Map<Produto>(model);
                    produto.ExtensaoImagem = extensao;
                    int idProduto =_produtoDAO.RegistarProduto(produto);
                    string pathAnterior = null;
                    string pathNova = Path.Combine("Images", "Produto", $"{idProduto}.{extensao}");
                    paths = new Tuple<string, string>(pathAnterior, pathNova);
                }
            }
            return new ServiceResult<Tuple<string, string>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = paths};
        }

        public ServiceResult<Tuple<string, string>> EditarProduto(EditarProdutoDTO model, string extensao)
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
            if (extensao == null)
            {
                throw new ArgumentNullException("Extensao", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(model.IdProduto);
            Tuple<string, string> paths = null;

            if (produto == null)
            {
                _logger.LogDebug($"Não existe nenhum produto com IdProduto {model.IdProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.isAtivo(model.IdProduto))
                {
                    if (!produto.Nome.Equals(model.Nome) && _produtoDAO.ExisteNomeProduto(model.Nome))
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


                        if (!erros.Any())
                        {
                            Produto novoProduto = _mapper.Map<Produto>(model);
                            novoProduto.ExtensaoImagem = extensao;
                            _produtoDAO.EditarProduto(novoProduto);
                            string pathAnterior = Path.Combine("Images", "Produto", $"{model.IdProduto}.{produto.ExtensaoImagem}"); ;
                            string pathNova = Path.Combine("Images", "Produto", $"{model.IdProduto}.{novoProduto.ExtensaoImagem}");
                            paths = new Tuple<string, string>(pathAnterior, pathNova);
                        }
                    }
                }
                else
                {
                    _logger.LogDebug($"O produto com idProduto {model.IdProduto} encontra-se desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }
            return new ServiceResult<Tuple<string, string>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(),Resultado = paths };
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
 