using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using API.Data;
using API.Entities;

namespace API.Business
{
    public interface IProdutoService
    {
        public bool AddProduto(string nome, string nomeCategoria, double preco, IList<string> ingredientes, IList<string> alergenios);
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
            if(ingredientes is null)
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
                ProdutoDAO.AddProduto(produto);
                sucesso = true;
            }
            return sucesso;
        }
    }
}
