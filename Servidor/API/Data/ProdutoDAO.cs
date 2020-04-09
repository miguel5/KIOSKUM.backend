using System;
using System.Collections.Generic;
using API.Entities;

namespace API.Data
{
    public interface IProdutoDAO
    {
        bool ExisteNome(string nome);
        void AddProduto(Produto produto);
        IList<Produto> GetProdutos(int idCategoria);
        Produto GetProdutoId(int id);
        void EditarProduto(Produto produto);
    }

    public class ProdutoDAO : IProdutoDAO
    {
        

        public ProdutoDAO()
        {
        }

        bool IProdutoDAO.ExisteNome(string nome)
        {
            throw new NotImplementedException();
        }

        public void AddProduto(Produto produto)
        {
            throw new NotImplementedException();
        }

        public IList<Produto> GetProdutos(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public Produto GetProdutoId(int idProduto)
        {
            throw new NotImplementedException();
        }

        public void EditarProduto(Produto produto)
        {
            throw new NotImplementedException();
        }
    }
}
