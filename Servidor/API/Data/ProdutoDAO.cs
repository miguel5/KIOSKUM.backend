using System;
using System.Collections.Generic;
using API.Business;
using API.Entities;

namespace API.Data
{
    public interface IProdutoDAO
    {
        bool ExisteNomeProduto(string nome);
        void AddProduto(Produto produto);
        IList<Produto> GetProdutos(int idCategoria);
        void EditarProduto(Produto produto);
        Produto GetProdutoNome(string nome);
        void RemoverProduto(string nome);
    }

    public class ProdutoDAO : IProdutoDAO
    {
        private readonly IConnectionDB _connectionDB;

        public ProdutoDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        bool IProdutoDAO.ExisteNomeProduto(string nome)
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

        public void EditarProduto(Produto produto)
        {
            throw new NotImplementedException();
        }

        public Produto GetProdutoNome(string nome)
        {
            throw new NotImplementedException();
        }

        public void RemoverProduto(string nome)
        {
            throw new NotImplementedException();
        }
    }
}
