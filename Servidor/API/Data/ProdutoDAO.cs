using System;
using System.Collections.Generic;
using System.Data;
using API.Data.Interfaces;
using API.Entities;
using API.Services.DBConnection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public class ProdutoDAO : IProdutoDAO
    {
        private readonly ILogger _logger;
        private readonly IConnectionDBService _connectionDBService;

        public ProdutoDAO(ILogger<ProdutoDAO> logger, IConnectionDBService connectionDBService)
        {
            _logger = logger;
            _connectionDBService = connectionDBService;
        }

        public void AtivarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> AtivarProduto]");

            _connectionDBService.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "ativar_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDBService.CloseConnection();
        }

        public void DesativarProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> DesativarProduto]");

            _connectionDBService.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "desativar_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDBService.CloseConnection();
        }

        public void EditarProduto(Produto produto)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> EditarProduto]");

            _connectionDBService.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

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

            _connectionDBService.CloseConnection();
        }

        public bool ExisteNomeProduto(string nome)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> ExisteNomeProduto]");

            _connectionDBService.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "existe_nome_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDBService.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public bool ExisteProduto(int idProduto)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> ExisteProduto]");
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "existe_produto";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDBService.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public Produto GetProduto(int idProduto)
        {

            _logger.LogDebug("A executar [ProdutoDAO -> GetProduto]");


            _connectionDBService.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

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
                _connectionDBService.CloseConnection();
            }
        }

        public Produto GetProdutoNome(string nome)
        {
            _logger.LogDebug("A executar [ProdutoDAO -> GetProdutoNome]");

            _connectionDBService.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            MySqlCommand cmdI = new MySqlCommand();
            MySqlCommand cmdA = new MySqlCommand();

            cmd.Connection = _connectionDBService.Connection;

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

                    var.Close();
                    cmdI.Connection = _connectionDBService.Connection;
                    cmdI.CommandText = "get_ingredientes_produto";
                    cmdI.CommandType = CommandType.StoredProcedure;

                    cmdI.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmdI.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmdI.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Ingredientes.Add(var.GetString(0));
                    }
                    var.Close();
                    cmdA.Connection = _connectionDBService.Connection;


                    cmdA.CommandText = "get_alergenicos_produto";
                    cmdA.CommandType = CommandType.StoredProcedure;

                    cmdA.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmdA.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmdA.ExecuteReader();

                    while (var.Read())
                    {
                        produto.Alergenios.Add(var.GetString(0));
                    }
                    var.Close();
                }
                return produto;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public IList<Produto> GetProdutosDesativados()
        {
            _logger.LogDebug("A executar [ProdutoDAO -> GetProdutosDesativados]");

            IList<Produto> produtos = new List<Produto>();

            _connectionDBService.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

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
                _connectionDBService.CloseConnection();
            }

        }

        public bool IsAtivo(int idProduto)
        {
            _connectionDBService.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "is_produto_ativo";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDBService.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public int RegistarProduto(Produto produto)
        {
            _connectionDBService.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

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
                MySqlCommand cmdI = new MySqlCommand();
                cmdI.Connection = _connectionDBService.Connection;

                cmdI.CommandText = "adicionar_ingrediente";
                cmdI.CommandType = CommandType.StoredProcedure;

                cmdI.Parameters.AddWithValue("?nome", ingrediente);
                cmdI.Parameters["?nome"].Direction = ParameterDirection.Input;

                int ingredientId = Convert.ToInt32(cmdI.ExecuteScalar());

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
                cmdA.Connection = _connectionDBService.Connection;

                cmdA.CommandText = "adicionar_alergenico";
                cmdA.CommandType = CommandType.StoredProcedure;

                cmdA.Parameters.AddWithValue("?nome", alergenico);
                cmdA.Parameters["?nome"].Direction = ParameterDirection.Input;

                int alergenicId = Convert.ToInt32(cmd.ExecuteScalar());

                cmdA.CommandText = "adicionar_produto_alergenico";
                cmdA.CommandType = CommandType.StoredProcedure;

                cmdA.Parameters.AddWithValue("?produto", productId);
                cmdA.Parameters["?produto"].Direction = ParameterDirection.Input;

                cmdA.Parameters.AddWithValue("?alergenico", alergenicId);
                cmdA.Parameters["?alergenico"].Direction = ParameterDirection.Input;

                cmdA.ExecuteNonQuery();

            }
            _connectionDBService.CloseConnection();
            return productId;
        }
    }
}
