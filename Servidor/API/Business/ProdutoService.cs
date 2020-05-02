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

namespace API.Business
{
    public interface IProdutoService
    {
        ServiceResult RegistarProduto(RegistarProdutoDTO model);
        Task<ServiceResult> UploadImagem(int idProduto, FormFile ficheiro);
        ServiceResult EditarProduto(EditarProdutoDTO model);
        ServiceResult<IList<ProdutoViewDTO>> GetProdutosCategoria(int idCategoria);
        ServiceResult<ProdutoViewDTO> GetProduto(int idProduto);
        ServiceResult DesativarProduto(int idProduto);
    }


    public class ProdutoService : IProdutoService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        private readonly IProdutoDAO _produtoDAO;
        private readonly ICategoriaDAO _categoriaDAO;


        public ProdutoService(IWebHostEnvironment webHostEnviroment, IMapper mapper, IOptions<AppSettings> appSettings, IProdutoDAO produtoDAO, ICategoriaDAO categoriaDAO)
        {
            _webHostEnvironment = webHostEnviroment;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _produtoDAO = produtoDAO;
            _categoriaDAO = categoriaDAO;
        }


        private bool ValidaPreco(double preco)
        {
            Regex rx = new Regex("^\\d{1,6}(.\\d{1,2})?$");
            return rx.IsMatch(preco.ToString());
        }


        public ServiceResult RegistarProduto(RegistarProdutoDTO model)
        {
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

            if (_produtoDAO.ExisteNomeProduto(model.Nome))
            {
                erros.Add((int)ErrosEnumeration.NomeProdutoJaExiste);
            }
            if (!ValidaPreco(model.Preco))
            {
                erros.Add((int)ErrosEnumeration.PrecoInvalido);
            }

            if (!_categoriaDAO.ExisteCategoria(model.IdCategoria))
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }

            if (!erros.Any())
            {
                Produto produto = _mapper.Map<Produto>(model);
                _produtoDAO.RegistarProduto(produto);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public async Task<ServiceResult> UploadImagem(int idProduto, FormFile ficheiro)
        {
            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                string fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(ficheiro.ContentDisposition).FileName);
                if (fileExtension.Contains('.'))
                {
                    fileExtension = fileExtension.Trim('"').Trim('.');
                }
                else
                {
                    erros.Add((int)ErrosEnumeration.FormatoImagemInvalido);
                }

                if (!erros.Any())
                {

                    if (Enum.IsDefined(typeof(ExtensoesValidasEnumeration), fileExtension))
                    {
                        if (ficheiro.Length > 0)
                        {
                            string extensaoAnterior = produto.ExtensaoImagem;
                            produto.ExtensaoImagem = fileExtension;
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produto", $"{idProduto}.{produto.ExtensaoImagem}");
                            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                            await ficheiro.CopyToAsync(fileStream);
                            _produtoDAO.EditarProduto(produto);

                            if (!produto.ExtensaoImagem.Equals(extensaoAnterior))
                            {
                                filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produto", $"{idProduto}.{extensaoAnterior}");
                                await Task.Factory.StartNew(() => File.Delete(filePath));
                            }
                        }
                        else
                        {
                            erros.Add((int)ErrosEnumeration.ImagemVazia);
                        }
                    }
                    else
                    {
                        erros.Add((int)ErrosEnumeration.FormatoImagemInvalido);
                    }
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult EditarProduto(EditarProdutoDTO model)
        {
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
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                if (!produto.Nome.Equals(model.Nome))
                {
                    erros.Add((int)ErrosEnumeration.NomeProdutoJaExiste);
                }

                if (!ValidaPreco(model.Preco))
                {
                    erros.Add((int)ErrosEnumeration.PrecoInvalido);
                }
                
                if (!_categoriaDAO.ExisteCategoria(model.IdCategoria))
                {
                    erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
                }

                if (!erros.Any())
                {
                    Produto novoProduto = _mapper.Map<Produto>(model);
                    _produtoDAO.EditarProduto(novoProduto);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<IList<ProdutoViewDTO>> GetProdutosCategoria(int idCategoria)
        {
            IList<int> erros = new List<int>();
            IList<ProdutoViewDTO> produtosDTO = null;

            if(_categoriaDAO.ExisteCategoria(idCategoria))
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                IList<Produto> produtos = _produtoDAO.GetProdutosCategoria(idCategoria);
                if (produtos != null)
                {
                    string pathImagem = Path.Combine(_appSettings.ServerUrl, "Images", "Produto");
                    foreach (Produto produto in produtos)
                    {
                        ProdutoViewDTO produtoDTO = _mapper.Map<ProdutoViewDTO>(produto);
                        produtoDTO.Url = new Uri(Path.Combine(pathImagem, $"{produto.IdProduto}.{produto.ExtensaoImagem}"));
                        produtosDTO.Add(produtoDTO);
                    }
                }
            }
            return new ServiceResult<IList<ProdutoViewDTO>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = produtosDTO };
        }


        public ServiceResult<ProdutoViewDTO> GetProduto(int idProduto)
        {
            IList<int> erros = new List<int>();
            ProdutoViewDTO produtoDTO = null;

            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                produtoDTO = _mapper.Map<ProdutoViewDTO>(produto);
                produtoDTO.Url = new Uri(Path.Combine(_appSettings.ServerUrl, "Images", "Produto", $"{produto.IdProduto}.{produto.ExtensaoImagem}"));
            }

            return new ServiceResult<ProdutoViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = produtoDTO };
        }


        public ServiceResult DesativarProduto(int idProduto)
        {
            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                _produtoDAO.DesativarProduto(idProduto);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        /*public ServiceResult AtivarProduto(int idProduto)
        {
            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProduto(idProduto);

            if (produto == null)
            {
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                _produtoDAO.DesativarProduto(idProduto);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }*/
    }
}
 