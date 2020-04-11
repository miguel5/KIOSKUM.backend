using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace API.Business
{
    public interface IProdutoService
    {
        ServiceResult AddProduto(ProdutoDTO model);
        Task<ServiceResult> UploadImagem(ImagemDTO model);
        ServiceResult EditarProduto(ProdutoDTO model);
        ServiceResult<IList<ProdutoDTO>> GetProdutosCategoria(string nomeCategoria);
        ServiceResult<ProdutoDTO> GetProdutoNome(string nome);
    }


    public class ProdutoService : IProdutoService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IProdutoDAO _produtoDAO;
        private readonly ICategoriaDAO _categoriaDAO;


        public ProdutoService(IWebHostEnvironment webHostEnviroment, IMapper mapper, IProdutoDAO produtoDAO, ICategoriaDAO categoriaDAO)
        {
            _webHostEnvironment = webHostEnviroment;
            _mapper = mapper;
            _produtoDAO = produtoDAO;
            _categoriaDAO = categoriaDAO;
        }


        private bool ValidaPreco(double preco)
        {
            Regex rx = new Regex("^\\d{1,6}(.\\d{1,2})?$");
            return rx.IsMatch(preco.ToString());
        }


        public ServiceResult AddProduto(ProdutoDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(model.NomeCategoria))
            {
                throw new ArgumentNullException("NomeCategoria", "Parametro não pode ser nulo");
            }
            if(model.Ingredientes == null)
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

            int idCategoria = _categoriaDAO.GetIdCategoria(model.NomeCategoria);

            if(idCategoria < 0)
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }

            if (!erros.Any())
            {
                Produto produto = _mapper.Map<Produto>(model);
                produto.IdCategoria = idCategoria;
                _produtoDAO.AddProduto(produto);
            }
            return new ServiceResult{ Erros = new ErrosDTO { Erros = erros } , Sucesso = !erros.Any()};
        }


        public async Task<ServiceResult> UploadImagem(ImagemDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            Produto produto = _produtoDAO.GetProdutoNome(model.Nome);

            if (produto == null)
            {
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                string fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(model.File.ContentDisposition).FileName);
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
                        if (model.File.Length > 0)
                        {
                            produto.ExtensaoImagem = fileExtension;
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produto", produto.Nome + "." + produto.ExtensaoImagem);
                            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                            await model.File.CopyToAsync(fileStream);
                            _produtoDAO.EditarProduto(produto);
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
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros },Sucesso = !erros.Any() };
        }


        public ServiceResult EditarProduto(ProdutoDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(model.NomeCategoria))
            {
                throw new ArgumentNullException("NomeCategoria", "Parametro não pode ser nulo");
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
            Produto produto = _produtoDAO.GetProdutoNome(model.Nome);

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

                int idCategoria = _categoriaDAO.GetIdCategoria(model.NomeCategoria);
                if (idCategoria < 0)
                {
                    erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
                }

                if (!erros.Any())
                {
                    Produto p = _mapper.Map<Produto>(model);
                    p.IdCategoria = idCategoria;
                    _produtoDAO.EditarProduto(p);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<IList<ProdutoDTO>> GetProdutosCategoria(string nomeCategoria)
        {
            if (string.IsNullOrWhiteSpace(nomeCategoria))
            {
                throw new ArgumentNullException("nomeCategoria", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            IList<ProdutoDTO> produtosDTO = null;

            int idCategoria = _categoriaDAO.GetIdCategoria(nomeCategoria);
            if(idCategoria < 0)
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                IList<Produto> produtos = _produtoDAO.GetProdutos(idCategoria);
                if (produtos != null)
                {
                    string pathImagem = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produto");
                    foreach (Produto produto in produtos)
                    {
                        ProdutoDTO produtoDTO = _mapper.Map<ProdutoDTO>(produto);
                        produtoDTO.Url = new System.Security.Policy.Url(Path.Combine(pathImagem, produto.Nome + "." + produto.ExtensaoImagem));
                    }
                }
            }
            return new ServiceResult<IList<ProdutoDTO>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = produtosDTO };
        }


        public ServiceResult<ProdutoDTO> GetProdutoNome(string nome)
        {
            IList<int> erros = new List<int>();
            ProdutoDTO produtoDTO = null;

            Produto produto = _produtoDAO.GetProdutoNome(nome);

            if (produto == null)
            {
                erros.Add((int)ErrosEnumeration.ProdutoNaoExiste);
            }
            else
            {
                produtoDTO = _mapper.Map<ProdutoDTO>(produto);
                string pathImagem = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produto");
                produtoDTO.Url = new System.Security.Policy.Url(Path.Combine(pathImagem, produto.IdProduto + "." + produto.ExtensaoImagem));
            }

            return new ServiceResult<ProdutoDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = produtoDTO };
        }
    }
}