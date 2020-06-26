using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Business.Interfaces;
using DAO.Interfaces;
using Entities;
using Helpers;
using DTO;
using DTO.CategoriaDTOs;
using DTO.ProdutoDTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;

namespace Business
{
    public class CategoriaBusiness : ICategoriaBusiness
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly ICategoriaDAO _categoriaDAO;

        public CategoriaBusiness(ILogger<CategoriaBusiness> logger, IOptions<AppSettings> appSettings, IMapper mapper, ICategoriaDAO categoriaDAO)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _categoriaDAO = categoriaDAO;
        }


        public ServiceResult<Tuple<string, string>> RegistarCategoria(RegistarCategoriaDTO model, string extensao)
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> RegistarCategoria]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }
            if (extensao is null)
            {
                throw new ArgumentNullException("Extensao", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Tuple<string, string> paths = null;

            if (_categoriaDAO.ExisteNomeCategoria(model.Nome))
            {
                _logger.LogDebug($"A Categoria com o Nome {model.Nome} já existe.");
                Categoria categoria = _categoriaDAO.GetCategoriaNome(model.Nome);
                if (_categoriaDAO.IsAtiva(categoria.IdCategoria))
                {
                    _logger.LogDebug($"A Categoria com o Nome {model.Nome} já existe, com IdCategoria {categoria.IdCategoria} e encontra-se ativada.");
                    erros.Add((int)ErrosEnumeration.NomeCategoriaJaExiste);
                }
                else
                {
                    _logger.LogDebug($"A Categoria com o Nome {model.Nome} já existe, com IdCategoria {categoria.IdCategoria} e encontra-se desativada.");
                    erros.Add((int)ErrosEnumeration.CategoriaDesativada);
                }
            }
            else
            {
                if (!ValidaNome(model.Nome))
                {
                    _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                    erros.Add((int)ErrosEnumeration.NomeCategoriaInvalido);
                }


                if (!erros.Any())
                {
                    Categoria categoria = _mapper.Map<Categoria>(model);
                    categoria.ExtensaoImagem = extensao;
                    int idCategoria = _categoriaDAO.RegistarCategoria(categoria);
                    string pathAnterior = null;
                    string pathNova = Path.Combine("Images", "Categoria", $"{idCategoria}.{extensao}");
                    paths = new Tuple<string, string>(pathAnterior, pathNova);
                }
            }
            return new ServiceResult<Tuple<string, string>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = paths };
        }


        public ServiceResult<Tuple<string, string>> EditarCategoria(EditarCategoriaDTO model, string extensao)
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> EditarCategoria]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }
            if (extensao is null)
            {
                throw new ArgumentNullException("Extensao", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Categoria categoria = _categoriaDAO.GetCategoria(model.IdCategoria);
            Tuple<string, string> paths = null;

            if (categoria == null)
            {
                _logger.LogWarning($"Não existe nenhuma Categoria com IdCategoria {model.IdCategoria}!");
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                if (_categoriaDAO.IsAtiva(model.IdCategoria))
                {
                    if (!categoria.Nome.Equals(model.Nome) && _categoriaDAO.ExisteNomeCategoria(model.Nome))
                    {

                        _logger.LogDebug($"O Nome {model.Nome} já existe.");
                        erros.Add((int)ErrosEnumeration.NomeCategoriaJaExiste);
                    }
                    else
                    {
                        if (!ValidaNome(model.Nome))
                        {
                            _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                            erros.Add((int)ErrosEnumeration.NomeCategoriaInvalido);
                        }

                        if (!erros.Any())
                        {
                            Categoria novaCategoria = _mapper.Map<Categoria>(model);
                            novaCategoria.ExtensaoImagem = extensao;
                            _categoriaDAO.EditarCategoria(novaCategoria);
                            string pathAnterior = Path.Combine("Images", "Categoria", $"{model.IdCategoria}.{categoria.ExtensaoImagem}"); ;
                            string pathNova = Path.Combine("Images", "Categoria", $"{model.IdCategoria}.{novaCategoria.ExtensaoImagem}");
                            paths = new Tuple<string, string>(pathAnterior, pathNova);
                        }
                    }
                }
                else
                {
                    _logger.LogDebug($"A Categoria com IdCategoria {model.IdCategoria} encontra-se desativada.");
                    erros.Add((int)ErrosEnumeration.CategoriaDesativada);
                }
            }
            return new ServiceResult<Tuple<string, string>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = paths };
        }



        public IList<CategoriaViewDTO> GetCategoriasDesativadas()
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> GetCategoriasDesativadas]");
            IList<CategoriaViewDTO> categoriasViewDTO = null;

            IList<Categoria> categorias = _categoriaDAO.GetCategoriasDesativadas();
            if (categorias != null)
            {
                string pathImagem = Path.Combine(_appSettings.ServerUrl, "Images", "Categoria");
                foreach (Categoria categoria in categorias)
                {
                    Console.WriteLine(categoria.ExtensaoImagem);
                    CategoriaViewDTO categoriaViewDTO = _mapper.Map<CategoriaViewDTO>(categoria);
                    categoriaViewDTO.Url = new Uri(Path.Combine(pathImagem, $"{categoria.IdCategoria}.{categoria.ExtensaoImagem}"));
                    categoriasViewDTO.Add(categoriaViewDTO);
                }
            }
            else
            {
                categoriasViewDTO = new List<CategoriaViewDTO>();
            }

            return categoriasViewDTO;
        }



        public IList<CategoriaViewDTO> GetCategorias()
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> GetCategoriasAtivadas]");
            IList<CategoriaViewDTO> categoriasViewDTO = new List<CategoriaViewDTO>();

            IList<Categoria> categorias = _categoriaDAO.GetCategorias();
            if (categorias != null)
            {
                string pathImagem = Path.Combine(_appSettings.ServerUrl, "Images", "Categoria");
                foreach (Categoria categoria in categorias)
                {
                    CategoriaViewDTO categoriaViewDTO = _mapper.Map<CategoriaViewDTO>(categoria);
                    categoriaViewDTO.Url = new Uri(Path.Combine(pathImagem, $"{categoria.IdCategoria}.{categoria.ExtensaoImagem}"));
                    categoriasViewDTO.Add(categoriaViewDTO);
                }
            }

            return categoriasViewDTO;
        }


        public ServiceResult<IList<ProdutoViewDTO>> GetProdutosCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> GetProdutosCategoria]");
            IList<int> erros = new List<int>();
            IList<ProdutoViewDTO> produtosViewDTO = null;

            if (!_categoriaDAO.ExisteCategoria(idCategoria))
            {
                _logger.LogWarning($"A Categoria com IdCategoria {idCategoria} não existe!");
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                if (_categoriaDAO.IsAtiva(idCategoria))
                {
                    IList<Produto> produtos = _categoriaDAO.GetProdutosCategoria(idCategoria);
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
                }
                else
                {
                    _logger.LogDebug($"A Categoria com IdCategoria {idCategoria} encontra-se desativada!");
                    erros.Add((int)ErrosEnumeration.CategoriaDesativada);
                }
            }

            return new ServiceResult<IList<ProdutoViewDTO>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = produtosViewDTO };
        }



        public ServiceResult<CategoriaViewDTO> GetCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> GetCategoria]");
            IList<int> erros = new List<int>();
            CategoriaViewDTO categoriaViewDTO = null;

            Categoria categoria = _categoriaDAO.GetCategoria(idCategoria);

            if (categoria == null)
            {
                _logger.LogWarning($"Não existe nenhuma Categoria com IdCategoria {idCategoria}!");
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                if (_categoriaDAO.IsAtiva(idCategoria))
                {
                    categoriaViewDTO = _mapper.Map<CategoriaViewDTO>(categoria);
                    categoriaViewDTO.Url = new Uri(Path.Combine(_appSettings.ServerUrl, "Images", "Categoria", $"{categoria.IdCategoria}.{categoria.ExtensaoImagem}"));
                }
                else
                {
                    _logger.LogDebug($"A Categoria com IdCategoria {idCategoria} encontra-se desativada!");
                    erros.Add((int)ErrosEnumeration.CategoriaDesativada);
                }
            }

            return new ServiceResult<CategoriaViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = categoriaViewDTO };
        }


        public ServiceResult DesativarCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> DesativarCategoria]");
            IList<int> erros = new List<int>();
            Categoria categoria = _categoriaDAO.GetCategoria(idCategoria);

            if (categoria == null)
            {
                _logger.LogWarning($"Não existe nenhuma Categoria com IdCategoria {idCategoria}!");
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                if (_categoriaDAO.IsAtiva(idCategoria))
                {
                    int numProdutosAtivados = _categoriaDAO.GetNumProdutosAtivados(idCategoria);
                    if (numProdutosAtivados == 0)
                    {
                        _categoriaDAO.DesativarCategoria(idCategoria);
                    }
                    else
                    {
                        _logger.LogDebug($"A Categoria com IdCategoria {idCategoria} possuí {numProdutosAtivados} ativados!");
                        erros.Add((int)ErrosEnumeration.ExistemProdutosAtivados);
                    }
                }
                else
                {
                    _logger.LogDebug($"A Categoria com IdCategoria {idCategoria} já se encontra desativada!");
                    erros.Add((int)ErrosEnumeration.CategoriaDesativada);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult AtivarCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> AtivarCategoria]");
            IList<int> erros = new List<int>();
            Categoria categoria = _categoriaDAO.GetCategoria(idCategoria);

            if (categoria == null)
            {
                _logger.LogWarning($"Não existe nenhuma Categoria com IdCategoria {idCategoria}!");
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                if (_categoriaDAO.IsAtiva(idCategoria))
                {
                    _logger.LogDebug($"A Categoria com IdCategoria {idCategoria} já se encontra ativada!");
                    erros.Add((int)ErrosEnumeration.CategoriaAtivada);
                }
                else
                {
                    _categoriaDAO.AtivarCategoria(idCategoria);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }



        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [CategoriaBusiness -> ValidaNome]");
            return nome.Length <= 45;
        }
    }
}