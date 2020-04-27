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
            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "adicionar_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", produto.Nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?preco", produto.Preco);
            cmd.Parameters["?preco"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?categoria", produto.IdCategoria);
            cmd.Parameters["?categoria"].Direction = ParameterDirection.Input;

            int productId = Convert.ToInt32(cmd.ExecuteScalar());

            
            foreach (string ingrediente in produto.Ingredientes)
            {
                MySqlCommand cmdI = new MySqlCommand();
                cmdI.Connection = _connectionDB.Connection;
                cmdI.CommandText = "adicionar_ingrediente";
                cmdI.CommandType = CommandType.StoredProcedure;

                cmdI.Parameters.AddWithValue("?nome", ingrediente);
                cmdI.Parameters["?nome"].Direction = ParameterDirection.Input;

                int ingredientId = (int)cmdI.ExecuteScalar();

                cmdI.CommandText = "adicionar_produto_ingrediente";
                cmdI.CommandType = CommandType.StoredProcedure;

                cmdI.Parameters.AddWithValue("?produto", productId);
                cmdI.Parameters["?produto"].Direction = ParameterDirection.Input;

                cmdI.Parameters.AddWithValue("?ingrediente", ingredientId);
                cmdI.Parameters["?ingrediente"].Direction = ParameterDirection.Input;

                cmdI.ExecuteNonQuery();

            }

            foreach (string alergenico in produto.Alergenios)
            {
                MySqlCommand cmdA = new MySqlCommand();
                cmdA.Connection = _connectionDB.Connection;
                cmdA.CommandText = "adicionar_alergenico";
                cmdA.CommandType = CommandType.StoredProcedure;

                cmdA.Parameters.AddWithValue("?nome", alergenico);
                cmdA.Parameters["?nome"].Direction = ParameterDirection.Input;

                int alergenicId = (int)cmdA.ExecuteScalar();

                cmdA.CommandText = "adicionar_produto_alergenico";
                cmdA.CommandType = CommandType.StoredProcedure;

                cmdA.Parameters.AddWithValue("?produto", productId);
                cmdA.Parameters["?produto"].Direction = ParameterDirection.Input;

                cmdA.Parameters.AddWithValue("?alergenico", alergenicId);
                cmdA.Parameters["?alergenico"].Direction = ParameterDirection.Input;

                cmdA.ExecuteNonQuery();

            }

            _connectionDB.CloseConnection();
        }


        public IList<Produto> GetProdutos(int idCategoria)
        {
            IList<Produto> produtos = new List<Produto>();

            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_produtos";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?idCategoria", idCategoria);
            cmd.Parameters["?idCategoria"].Direction = ParameterDirection.Input;

            MySqlDataReader var = cmd.ExecuteReader();
            try
            {
                while (var.Read())
                {
                    Produto produto = new Produto { IdProduto = var.GetInt32(0), Nome = var.GetString(1), IdCategoria = idCategoria, Preco = var.GetDouble(2), Ingredientes = new List<string>(), Alergenios = new List<string>(), ExtensaoImagem = var.GetString(3) };

                    MySqlCommand cmdI = new MySqlCommand();
                    cmdI.Connection = _connectionDB.Connection;

                    cmdI.CommandText = "get_ingredientes_produto";
                    cmdI.CommandType = CommandType.StoredProcedure;

                    cmdI.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmdI.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmdI.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Ingredientes.Add(var.GetString(0));
                    }

                    MySqlCommand cmdA = new MySqlCommand();
                    cmdA.Connection = _connectionDB.Connection;

                    cmdA.CommandText = "get_alergenicos_produto";
                    cmdA.CommandType = CommandType.StoredProcedure;

                    cmdA.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmdA.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmdA.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Alergenios.Add(var.GetString(0));
                    }

                    produtos.Add(produto);
                }
            }
            finally
            {
                _connectionDB.CloseConnection();
            }

            return produtos;
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

            cmd.Parameters.AddWithValue("?extensao_imagem", produto.ExtensaoImagem);
            cmd.Parameters["?extensao_imagem"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?categoria", produto.IdCategoria);
            cmd.Parameters["?categoria"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDB.CloseConnection();
        }


        public Produto GetProdutoNome(string nome)
        {
            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_produto_nome";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            MySqlDataReader var = cmd.ExecuteReader();

            Produto produto = null;
            try
            {
                if (var.Read())
                {
                    produto = new Produto { IdProduto = var.GetInt32(0), Nome = nome, IdCategoria = var.GetInt32(3), Preco = var.GetDouble(1), Ingredientes = new List<string>(), Alergenios = new List<string>(), ExtensaoImagem = var.GetString(2) };

                    MySqlCommand cmdI = new MySqlCommand();
                    cmdI.Connection = _connectionDB.Connection;

                    cmdI.CommandText = "get_ingredientes_produto";
                    cmdI.CommandType = CommandType.StoredProcedure;

                    cmdI.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmdI.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var.Close();
                    var = cmdI.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Ingredientes.Add(var.GetString(0));
                    }

                    MySqlCommand cmdA = new MySqlCommand();
                    cmdA.Connection = _connectionDB.Connection;

                    cmdA.CommandText = "get_alergenicos_produto";
                    cmdA.CommandType = CommandType.StoredProcedure;

                    cmdA.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmdA.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var.Close();
                    var = cmdA.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Alergenios.Add(var.GetString(0));
                    }
                    var.Close();
                }
            }
            finally
            {
                _connectionDB.CloseConnection();
            }
            return produto;
        }

        public void RemoverProduto(string nome)
        {
            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "remover_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDB.CloseConnection();
        }
    }
}

