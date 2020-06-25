using System;
using System.Collections.Generic;
using System.Data;
using DAO.Interfaces;
using Entities;
using Services.DBConnection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DAO
{
    public class CategoriaDAO : ICategoriaDAO
    {
        private readonly ILogger _logger;
        private readonly IConnectionDBService _connectionDBService;

        public CategoriaDAO(ILogger<CategoriaDAO> logger, IConnectionDBService connectionDBService)
        {
            _logger = logger;
            _connectionDBService = connectionDBService;
        }

        public void AtivarCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> AtivarCategoria]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "ativar_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idCategoria);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public void DesativarCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> DesativarCategoria]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "desativar_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idCategoria);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public void EditarCategoria(Categoria novaCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> EditarCategoria]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "editar_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", novaCategoria.IdCategoria);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?nomes", novaCategoria.Nome);
                    cmd.Parameters["?nomes"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?extensao_imagems", novaCategoria.ExtensaoImagem);
                    cmd.Parameters["?extensao_imagems"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public bool ExisteCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> ExisteCategoria]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idCategoria);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return Convert.ToBoolean(val);
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public bool ExisteNomeCategoria(string nome)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> ExisteNomeCategoria]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_nome_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?nome", nome);
                    cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return Convert.ToBoolean(val);
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Categoria GetCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategoria]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idCategoria);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {

                        Categoria categoria = null;

                        if (var.Read())
                        {
                            categoria = new Categoria { IdCategoria = idCategoria, Nome = var.GetString(0), ExtensaoImagem = var.GetString(1) };
                        }
                        return categoria;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Categoria GetCategoriaNome(string nome)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategoriaNome]");

            try
            {
                _connectionDBService.OpenConnection();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_nome_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?nome", nome);
                    cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {
                        Categoria categoria = null;
                        if (var.Read())
                        {
                            categoria = new Categoria { IdCategoria = var.GetInt32(0), Nome = nome, ExtensaoImagem = var.GetString(1) };

                        }
                        return categoria;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public IList<Categoria> GetCategorias()
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategorias]");
            IList<Categoria> categorias = new List<Categoria>();

            try
            {
                _connectionDBService.OpenConnection();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_categorias";
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {

                        while (var.Read())
                        {
                            Categoria categoria = new Categoria { IdCategoria = var.GetInt32(0), Nome = var.GetString(1), ExtensaoImagem = var.GetString(3) };
                            categorias.Add(categoria);
                        }
                        return categorias;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public IList<Categoria> GetCategoriasDesativadas()
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategoriasDesativadas]");
            IList<Categoria> categorias = new List<Categoria>();
            try
            {
                _connectionDBService.OpenConnection();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_categorias_desativadas";
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {
                        while (var.Read())
                        {
                            Categoria categoria = new Categoria { IdCategoria = var.GetInt32(0), Nome = var.GetString(1), ExtensaoImagem = var.GetString(3) };
                            categorias.Add(categoria);
                        }
                        return categorias;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public int GetNumProdutosAtivados(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetNumProdutosAtivados]");
            try
            {
                _connectionDBService.OpenConnection();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_num_produtos_ativos";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idCategoria);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return Convert.ToInt32(val);
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public IList<Produto> GetProdutosCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetProdutosCategoria]");

            IList<Produto> produtos = new List<Produto>();
            try
            {
                _connectionDBService.OpenConnection();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_produtos";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idCategoria", idCategoria);
                    cmd.Parameters["?idCategoria"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {
                        while (var.Read())
                        {
                            Produto produto = new Produto { IdProduto = var.GetInt32(0), Nome = var.GetString(1), IdCategoria = idCategoria, Preco = var.GetDouble(2), Ingredientes = new List<string>(), Alergenios = new List<string>(), ExtensaoImagem = var.GetString(3) };

                            cmd.CommandText = "get_ingredientes_produto";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                            cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                            using (MySqlDataReader varI = cmd.ExecuteReader())
                            {
                                while (varI.Read())
                                {
                                    produto.Ingredientes.Add(varI.GetString(0));
                                }
                            }


                            cmd.CommandText = "get_alergenicos_produto";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                            cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                            using (MySqlDataReader varI = cmd.ExecuteReader())
                            {

                                while (var.Read())
                                {
                                    produto.Alergenios.Add(var.GetString(0));
                                }
                            }
                            produtos.Add(produto);
                        }
                        return produtos;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public bool IsAtiva(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> isAtiva]");

            _connectionDBService.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "is_categoria_ativa";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idCategoria);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDBService.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public int RegistarCategoria(Categoria categoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> RegistarCategoria]");

            try
            {
                _connectionDBService.OpenConnection();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "registar_categoria";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?nome", categoria.Nome);
                    cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?extensao_imagem", categoria.ExtensaoImagem);
                    cmd.Parameters["?extensao_imagem"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return Convert.ToInt32(val);
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }
    }
}