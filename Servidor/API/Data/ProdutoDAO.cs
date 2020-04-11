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
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "existe_nome_produto";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?nome", nome);
                cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                object val = cmd.ExecuteScalar();
                
                _connectionDB.CloseConnection();
                
                return Convert.ToBoolean(val);
            }
            return false;
        }

        public void AddProduto(Produto produto)
        {
            throw new NotImplementedException();
        }

        public IList<Produto> GetProdutos(int idCategoria)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "get_produtos";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?idCategoria", idCategoria);
                cmd.Parameters["?idCategoria"].Direction = ParameterDirection.Input;

                MySqlDataReader rdr = cmd.ExecuteReader();
                
                try
                {
                    while (rdr.Read())
                    {
                    }
                }
                catch (Exception) { }
                finally
                {
                    _connectionDB.CloseConnection();
                }
            }
            return null;
        }

        public void EditarProduto(Produto produto)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
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
        }

        public Produto GetProdutoNome(string nome)
        {
        }
    }
}
