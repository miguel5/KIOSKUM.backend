using System;
using System.Collections.Generic;
using System.Data;
using API.Business;
using API.Entities;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;
        private readonly IConnectionDB _connectionDB;

        public ProdutoDAO(ILogger<ProdutoDAO> logger, IConnectionDB connectionDB)
        {
            _logger = logger;
            _connectionDB = connectionDB;
        }

        public void AtivarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> AtivarProduto]");

            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "ativar_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDB.CloseConnection();
        }

        public void DesativarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> DesativarProduto]");

            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "desativar_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDB.CloseConnection();
        }

        public void EditarProduto(Produto produto)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> EditarProduto]");

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

        public bool ExisteNomeProduto(string nome)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> ExisteNomeProduto]");

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

        public Produto GetProduto(int idProduto)
        {

            _logger.LogDebug("A executar [ProdutoDAO -> GetProduto]");


            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            MySqlDataReader var = cmd.ExecuteReader();

            Produto produto = null;
            try
            {
                if (var.Read())
                {
                    produto = new Produto { IdProduto = idProduto, Nome = var.GetString(0), IdCategoria = var.GetInt32(3), Preco = var.GetDouble(1), Ingredientes = new List<string>(), Alergenios = new List<string>(), ExtensaoImagem = var.GetString(2) };

                    cmd.CommandText = "get_ingredientes_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Ingredientes.Add(var.GetString(0));
                    }

                    cmd.CommandText = "get_alergenicos_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Alergenios.Add(var.GetString(0));
                    }
                }
                return produto;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDB.CloseConnection();
            }
        }

        public Produto GetProdutoNome(string nome)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> GetProdutoNome]");

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

                    cmd.CommandText = "get_ingredientes_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Ingredientes.Add(var.GetString(0));
                    }

                    cmd.CommandText = "get_alergenicos_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Alergenios.Add(var.GetString(0));
                    }
                }
                return produto;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDB.CloseConnection();
            }
        }

        public IList<Produto> GetProdutosDesativados()
        {
            _logger.LogDebug("A executar [ProdutoDAO -> GetProdutosDesativados]");

            IList<Produto> produtos = new List<Produto>();

            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_produtos_desativados";
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataReader var = cmd.ExecuteReader();
            try
            {
                while (var.Read())
                {
                    Produto produto = new Produto { IdProduto = var.GetInt32(0), Nome = var.GetString(1), IdCategoria = var.GetInt32(4), Preco = var.GetDouble(2), Ingredientes = new List<string>(), Alergenios = new List<string>(), ExtensaoImagem = var.GetString(3) };

                    cmd.CommandText = "get_ingredientes_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Ingredientes.Add(var.GetString(0));
                    }

                    cmd.CommandText = "get_alergenicos_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Alergenios.Add(var.GetString(0));
                    }

                    produtos.Add(produto);
                }
                return produtos;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDB.CloseConnection();
            }

        }

        public bool isAtivo(int idProduto)
        {
            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "is_produto_ativo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDB.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public int RegistarProduto(Produto produto)
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

            cmd.Parameters.AddWithValue("?extensao_imagem", produto.ExtensaoImagem);
            cmd.Parameters["?extensao_imagem"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?categoria", produto.IdCategoria);
            cmd.Parameters["?categoria"].Direction = ParameterDirection.Input;

            int productId = Convert.ToInt32(cmd.ExecuteScalar());

            foreach (string ingrediente in produto.Ingredientes)
            {
                cmd.CommandText = "adicionar_ingrediente";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?nome", ingrediente);
                cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                int ingredientId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "adicionar_produto_ingrediente";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?produto", productId);
                cmd.Parameters["?produto"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?ingrediente", ingredientId);
                cmd.Parameters["?ingrediente"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

            }

            foreach (string alergenico in produto.Alergenios)
            {
                cmd.CommandText = "adicionar_alergenico";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?nome", alergenico);
                cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                int alergenicId = Convert.ToInt32(cmd.ExecuteScalar());

                cmd.CommandText = "adicionar_produto_alergenico";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?produto", productId);
                cmd.Parameters["?produto"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?alergenico", alergenicId);
                cmd.Parameters["?alergenico"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

            }
            _connectionDB.CloseConnection();
            return productId;
        }
    }
}
