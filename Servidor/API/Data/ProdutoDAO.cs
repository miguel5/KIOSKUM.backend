using System;
using System.Collections.Generic;
using System.Data;
using API.Business;
using API.Entities;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public interface IProdutoDAO
    {
        bool ExisteNomeProduto(string nome);
        int RegistarProduto(Produto produto);//retorna o idProduto
        Produto GetProduto(int idProduto);//devolve o produto (ativado/desativado)
        Produto GetProdutoNome(string nome);//devolve o produto (ativado/desativado)
        void EditarProduto(Produto produto);
        IList<Produto> GetProdutosDesativados();//apenas devolve os desativados
        void DesativarProduto(int idProduto);
        void AtivarProduto(int idProduto);
        bool isAtivo(int idProduto);
    }

    public class ProdutoDAO : IProdutoDAO
    {
        private readonly IConnectionDB _connectionDB;

        public ProdutoDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void AtivarProduto(int idProduto)
        {
            throw new NotImplementedException();
        }

        public void DesativarProduto(int idProduto)
        {
            throw new NotImplementedException();
        }

        public void EditarProduto(Produto produto)
        {
            throw new NotImplementedException();
        }

        public bool ExisteNomeProduto(string nome)
        {
            throw new NotImplementedException();
        }

        public Produto GetProduto(int idProduto)
        {
            throw new NotImplementedException();
        }

        public Produto GetProdutoNome(string nome)
        {
            throw new NotImplementedException();
        }

        public IList<Produto> GetProdutosDesativados()
        {
            throw new NotImplementedException();
        }

        public bool isAtivo(int idProduto)
        {
            throw new NotImplementedException();
        }

        public int RegistarProduto(Produto produto)
        {
            throw new NotImplementedException();
        }
    }
}
