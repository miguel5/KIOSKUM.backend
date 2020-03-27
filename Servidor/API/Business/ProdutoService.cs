using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;

namespace API.Business
{
    public interface IProdutoService
    {
        public bool AddProduto(string nome, string nomeCategoria, double preco, IList<string> ingredientes, IList<string> alergenios);
        public bool UploadImagem(IFormFile file);
    }


    public class ProdutoService : IProdutoService
    {
        private ProdutoDAO produtoDAO;
        private CategoriaDAO categoriaDAO;

        public ProdutoService()
        {
            produtoDAO = new ProdutoDAO();
            categoriaDAO = new CategoriaDAO();
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
            if (ingredientes is null)
            {
                throw new ArgumentNullException("Ingredientes", "Parametro não pode ser nulo");
            }
            if (alergenios is null)
            {
                throw new ArgumentNullException("Alergenios", "Parametro não pode ser nulo");
            }
            if (validaPreco(preco) && !produtoDAO.ExisteNome(nome))
            {
                int idCategoria = categoriaDAO.GetIdCategoria(nomeCategoria);
                Produto produto = new Produto { Nome = nome, IdCategoria = idCategoria, Preco = preco, Ingredientes = ingredientes, Alergenios = alergenios };
                produtoDAO.AddProduto(produto);
                sucesso = true;
            }
            return sucesso;
        }


        public bool UploadImagem(IFormFile file)
        {
            bool sucesso = false;
            string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            string fileExtension = Path.GetExtension(fileName.Trim('.'));
            if (fileExtension.Equals(".png"))
            {
                fileName = Path.Combine("/Users/lazaropinheiro/KIOSKUM.backend/Servidor/API/wwwroot/Images/Produtos", "1.png");

                using (FileStream fs = File.Create(fileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                sucesso = true;
            }
            return sucesso;
        }
    }

}
