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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;

namespace API.Business
{
    public interface IProdutoService
    {
        public IList<int> AddProduto(string nome, string nomeCategoria, double preco, IList<string> ingredientes, IList<string> alergenios);
        Task<IList<int>> UploadImagem(int IdProduto, IFormFile file);
    }


    public class ProdutoService : IProdutoService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageSettings _imageSettings;
        private IProdutoDAO _produtoDAO;
        private ICategoriaDAO _categoriaDAO;

        public ProdutoService(IOptions<AppSettings> appSettings, IWebHostEnvironment webHostEnviroment, IProdutoDAO produtoDAO, ICategoriaDAO categoriaDAO)
        {
            _webHostEnvironment = webHostEnviroment;
            _imageSettings = appSettings.Value.ImageSettings;
            _produtoDAO = produtoDAO;
            _categoriaDAO = categoriaDAO;
        }


        public bool validaPreco(double preco)
        {
            Regex rx = new Regex("^\\d{1,6}(.\\d{1,2})?$");
            return rx.IsMatch(preco.ToString());
        }


        public IList<int> AddProduto(string nome, string nomeCategoria, double preco, IList<string> ingredientes, IList<string> alergenios)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(nomeCategoria))
            {
                throw new ArgumentNullException("NomeCategoria", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            if (!_produtoDAO.ExisteNome(nome))
            {
                erros.Add(Erros.NomeProdutoJaExiste);
            }

            if (!validaPreco(preco))
            {
                erros.Add(Erros.PrecoInvalido);
            }

            if (!erros.Any())
            { 
                int idCategoria = _categoriaDAO.GetIdCategoria(nomeCategoria);
                Produto produto = new Produto { Nome = nome, IdCategoria = idCategoria, Preco = preco, Ingredientes = ingredientes, Alergenios = alergenios };
                _produtoDAO.AddProduto(produto);
            }
            return erros;
        }


        public async Task<IList<int>> UploadImagem(int IdProduto, IFormFile file)
        {
            IList<int> erros = new List<int>();
            if (!_produtoDAO.ExisteIdProduto(IdProduto))
            {
                erros.Add(Erros.ProdutoNaoExiste);
            }
            if(!erros.Any()){
                string fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').Trim('.'));
                if (fileExtension.Equals("." + _imageSettings.Extensao))
                {
                    if (file.Length > 0)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produtos", IdProduto + "." + _imageSettings.Extensao);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                }
            }
            return erros;
        }





    }

}
