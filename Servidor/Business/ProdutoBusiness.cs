using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Business.Interfaces;
using DAO.Interfaces;
using Entities;
using Helpers;
using DTO;
using DTO.ProdutoDTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;

namespace Business
{
    public class ProdutoBusiness : IProdutoBusiness
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IProdutoDAO _produtoDAO;
        private readonly ICategoriaDAO _categoriaDAO;


        public ProdutoBusiness(ILogger<ProdutoBusiness> logger, IOptions<AppSettings> appSettings, IMapper mapper, IProdutoDAO produtoDAO, ICategoriaDAO categoriaDAO)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _produtoDAO = produtoDAO;
            _categoriaDAO = categoriaDAO;
        }


        public ServiceResult<Tuple<string, string>> RegistarProduto(RegistarProdutoDTO model, string extensao)
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> RegistarProduto]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }
            if (model.Ingredientes == null)
            {
                throw new ArgumentNullException("Ingredientes", "Campo não poder ser nulo!");
            }
            if (model.Alergenios == null)
            {
                throw new ArgumentNullException("Alergenios", "Campo não poder ser nulo!");
            }
            if(extensao == null)
            {
                throw new ArgumentNullException("Extensao", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Tuple<string, string> paths = null;

            if (_produtoDAO.ExisteNomeProduto(model.Nome))
            {
                _logger.LogDebug($"O Produto com o Nome {model.Nome} já existe.");
                Produto produto = _produtoDAO.GetProdutoNome(model.Nome);
                if (_produtoDAO.IsAtivo(produto.IdProduto))
                {
                    _logger.LogDebug($"O Produto com o Nome {model.Nome} já existe, com IdProdudo {produto.IdProduto} e encontra-se ativado.");
                    erros.Add((int)ErrosEnumeration.NomeProdutoJaExiste);
                }
                else
                {
                    _logger.LogDebug($"O Produto com o Nome {model.Nome} já existe, com IdProdudo {produto.IdProduto} e encontra-se desativado.");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }
            else
            {
                if (!ValidaNome(model.Nome))
                {
                    _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                    erros.Add((int)ErrosEnumeration.NomeProdutoInvalido);
                }
                if (!ValidaPreco(model.Preco))
                {
                    _logger.LogDebug($"O Preço {model.Preco} é inválido.");
                    erros.Add((int)ErrosEnumeration.PrecoProdutoInvalido);
                }
                if (!_categoriaDAO.ExisteCategoria(model.IdCategoria))
                {
                    _logger.LogWarning($"Não existe nenhuma Categoria com IdCategooria {model.IdCategoria}!");
                    erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
                }
                if (!ValidaIngredientes(model.Ingredientes))
                {
                    _logger.LogDebug($"O nome de um ingrediente é inválido.");
                    erros.Add((int)ErrosEnumeration.IngredientesProdutoInvalidos);
                }
                if (!ValidaAlergenios(model.Alergenios))
                {
                    _logger.LogDebug($"O nome de um alergenio é inválido.");
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
            _logger.LogDebug("A executar [ProdutoBusiness -> EditarProduto]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }
            if (model.Ingredientes == null)
            {
                throw new ArgumentNullException("Ingredientes", "Campo não poder ser nulo!");
            }
            if (model.Alergenios == null)
            {
                throw new ArgumentNullException("Alergenios", "Campo não poder ser nulo!");
            }
            if (extensao == null)
            {
                throw new ArgumentNullException("Extensao", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(model.IdProduto);
            Tuple<string, string> paths = null;

            if (produto == null)
            {
                _logger.LogWarning($"Não existe nenhum Produto com IdProduto {model.IdProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.IsAtivo(model.IdProduto))
                {
                    if (!produto.Nome.Equals(model.Nome) && _produtoDAO.ExisteNomeProduto(model.Nome))
                    {
                        _logger.LogDebug($"O Nome {model.Nome} já existe.");
                        erros.Add((int)ErrosEnumeration.NomeProdutoJaExiste);
                    }
                    else
                    {
                        if (!ValidaNome(model.Nome))
                        {
                            _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                            erros.Add((int)ErrosEnumeration.NomeProdutoInvalido);
                        }
                        if (!ValidaPreco(model.Preco))
                        {
                            _logger.LogDebug($"O preço {model.Preco} é inválido.");
                            erros.Add((int)ErrosEnumeration.PrecoProdutoInvalido);
                        }
                        if (!_categoriaDAO.ExisteCategoria(model.IdCategoria))
                        {
                            _logger.LogWarning($"Não existe nenhuma Categoria, com IdCategooria {model.IdCategoria}!");
                            erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
                        }
                        if (!ValidaIngredientes(model.Ingredientes))
                        {
                            _logger.LogDebug($"O Nome de um ingrediente é inválido.");
                            erros.Add((int)ErrosEnumeration.IngredientesProdutoInvalidos);
                        }
                        if (!ValidaAlergenios(model.Alergenios))
                        {
                            _logger.LogDebug($"O Nome de um alergenio é inválido.");
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
                    _logger.LogDebug($"O Produto com idProduto {model.IdProduto} encontra-se desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }
            return new ServiceResult<Tuple<string, string>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(),Resultado = paths };
        }


        public IList<ProdutoViewDTO> GetProdutosDesativados()
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> GetProdutosDesativados]");
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
            _logger.LogDebug("A executar [ProdutoBusiness -> GetProduto]");
            IList<int> erros = new List<int>();
            ProdutoViewDTO produtoViewDTO = null;

            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                _logger.LogWarning($"Não existe nenhum Produto com IdProduto {idProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.IsAtivo(idProduto))
                {
                    produtoViewDTO = _mapper.Map<ProdutoViewDTO>(produto);
                    produtoViewDTO.Url = new Uri(Path.Combine(_appSettings.ServerUrl, "Images", "Produto", $"{produto.IdProduto}.{produto.ExtensaoImagem}"));
                }
                else
                {
                    _logger.LogDebug($"O Produto com IdProduto {idProduto} encontra-se desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }

            return new ServiceResult<ProdutoViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = produtoViewDTO };
        }


        public ServiceResult DesativarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> DesativarProduto]");

            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                _logger.LogWarning($"Não existe nenhum Produto com IdProduto {idProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.IsAtivo(idProduto))
                {
                    _produtoDAO.DesativarProduto(idProduto);
                }
                else
                {
                    _logger.LogDebug($"O Produto com IdProduto {idProduto} já se encontra desativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoDesativado);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult AtivarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> AtivarProduto]");
            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                _logger.LogWarning($"Não existe nenhum Produto com IdProduto {idProduto}!");
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (_produtoDAO.IsAtivo(idProduto))
                {
                    _logger.LogDebug($"O Produto com IdProduto {idProduto} já se encontra ativado!");
                    erros.Add((int)ErrosEnumeration.ProdutoAtivado);
                }
                else
                {
                    _produtoDAO.AtivarProduto(idProduto);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }



        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> ValidaNome]");
            return nome.Length <= 45;
        }

        private bool ValidaPreco(double preco)
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> ValidaPreco]");
            Regex rx = new Regex("^\\d{1,6}(.\\d{1,2})?$");
            return rx.IsMatch(preco.ToString());
        }

        private bool ValidaIngredientes(IList<string> ingredientes)
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> ValidaIngredientes]");
            bool sucesso = true;
            foreach (string ingrediente in ingredientes) if (sucesso)
                {
                    sucesso = ingrediente.Length <= 45;
                }
            return sucesso;
        }

        private bool ValidaAlergenios(IList<string> alergenios)
        {
            _logger.LogDebug("A executar [ProdutoBusiness -> ValidaAlergenios]");
            bool sucesso = true;
            foreach (string alergenio in alergenios) if (sucesso)
            {
                sucesso = alergenio.Length <= 45;
            }
            return sucesso;
        }
    }
}
 