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

        public bool ExisteNomeProduto(string nome)
        {
            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "existe_nome_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDB.CloseConnection();

            return Convert.ToBoolean(val);
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
            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "editar_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", produto.IdProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?nome", produto.Nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?preco", produto.Preco);
            cmd.Parameters["?preco"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?entensao_imagem", produto.ExtensaoImagem);
            cmd.Parameters["?entensao_imagem"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?categoria", produto.IdCategoria);
            cmd.Parameters["?categoria"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDB.CloseConnection();
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
