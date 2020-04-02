using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Business
{
    public interface IProdutoService
    {
        public bool AddProduto(string nome, string nomeCategoria, double preco, IList<string> ingredientes, IList<string> alergenios);
        Task<bool> UploadImagem(int IdProduto, IFormFile file);
    }


    public class ProdutoService : IProdutoService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppSettings _appSettings;
        private IProdutoDAO _produtoDAO;
        private ICategoriaDAO _categoriaDAO;

        public ProdutoService(IOptions<AppSettings> appSettings, IWebHostEnvironment webHostEnviroment, IProdutoDAO produtoDAO, ICategoriaDAO categoriaDAO)
        {
            _webHostEnvironment = webHostEnviroment;
            _appSettings = appSettings.Value;
            _produtoDAO = produtoDAO;
            _categoriaDAO = categoriaDAO;
        }


        public bool validaPreco(double preco)
        {
            Regex rx = new Regex("^\\d{1,6}(.\\d{1,2})?$");
            return rx.IsMatch(preco.ToString());
        }


        public bool AddProduto(string nome, string nomeCategoria, double preco, IList<string> ingredientes, IList<string> alergenios)
        {
            bool sucesso = false;
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(nomeCategoria))
            {
                throw new ArgumentNullException("NomeCategoria", "Parametro não pode ser nulo");
            }
            
            /*if (validaPreco(preco) && !_produtoDAO.ExisteNome(nome))
            {
                int idCategoria = _categoriaDAO.GetIdCategoria(nomeCategoria);
                Produto produto = new Produto { Nome = nome, IdCategoria = idCategoria, Preco = preco, Ingredientes = ingredientes, Alergenios = alergenios };
                _produtoDAO.AddProduto(produto);
                sucesso = true;
            }*/
            return sucesso;
        }


        public async Task<bool> UploadImagem(int IdProduto, IFormFile file)
        {
            bool sucesso = false;
            string fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"').Trim('.'));
            if (fileExtension.Equals(".png"))
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Produtos",IdProduto+".png");
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            return sucesso;
        }


       

    }

}
