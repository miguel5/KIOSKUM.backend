using System;
using System.Collections.Generic;
using System.Data;
using API.Business;
using API.Entities;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public interface ICategoriaDAO
    {
        bool ExisteCategoria(int idCategoria);//determina se o idCategoria ja se encontra no sistema
        bool ExisteNomeCategoria(string nome);//determina se o nome da categoria ja existe no sistema
        Categoria GetCategoriaNome(string nome);//devolve a categoria dando o nome
        bool IsAtiva(int idCategoria);//determina se uma categoria esta ou não ativa
        int RegistarCategoria(Categoria categoria);//devolve o id da categoria
        Categoria GetCategoria(int idCategoria);//Retorna uma categoria (ativada/desativada)
        void EditarCategoria(Categoria novaCategoria);//apenas edita se estiver ativada
        IList<Categoria> GetCategoriasDesativadas();//devolve todas as categorias desativada
        IList<Categoria> GetCategorias();//devolve todas as categorias ativadas
        IList<Produto> GetProdutosCategoria(int idCategoria);//devolve todos os produtos ativados de uma categoria
    }

    public class CategoriaDAO : ICategoriaDAO
    {
        private readonly ILogger _logger;
        private readonly IConnectionDB _connectionDB;

        public CategoriaDAO(ILogger<CategoriaDAO> logger ,IConnectionDB connectionDB)
        {
            _logger = logger;
            _connectionDB = connectionDB;
        }

        public void EditarCategoria(Categoria novaCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> EditarCategoria]");
            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "editar_categoria";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", novaCategoria.IdCategoria);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?nomes", novaCategoria.Nome);
            cmd.Parameters["?nomes"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?extensao_imagems", novaCategoria.ExtensaoImagem);
            cmd.Parameters["?extensao_imagems"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();
            Console.WriteLine(novaCategoria.Nome);
            Console.WriteLine(novaCategoria.ExtensaoImagem);
            _connectionDB.CloseConnection();
        }

        public bool ExisteCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> ExisteCategoria]");

            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "existe_categoria";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idCategoria);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDB.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public bool ExisteNomeCategoria(string nome)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> ExisteNomeCategoria]");

            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "existe_nome_categoria";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDB.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public Categoria GetCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategoria]");

            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_categoria";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idCategoria);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            MySqlDataReader var = cmd.ExecuteReader();

            Categoria categoria = null;
            try
            {
                if (var.Read())
                {
                    categoria = new Categoria { IdCategoria = idCategoria, Nome = var.GetString(0), ExtensaoImagem = var.GetString(1) };
                }
                return categoria;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDB.CloseConnection();
            }
        }

        public Categoria GetCategoriaNome(string nome)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategoriaNome]");
            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_nome_categoria";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            MySqlDataReader var = cmd.ExecuteReader();

            Categoria categoria = null;
            try
            {
                if (var.Read())
                {
                    categoria = new Categoria { IdCategoria = var.GetInt32(0), Nome = nome, ExtensaoImagem = var.GetString(1) };

                }
                return categoria;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDB.CloseConnection();
            }
        }

        public IList<Categoria> GetCategorias()
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategorias]");
            IList<Categoria> categorias = new List<Categoria>();

            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_categorias";
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataReader var = cmd.ExecuteReader();

            try
            {
                while (var.Read())
                {
                    Categoria categoria = new Categoria { IdCategoria = var.GetInt32(0), Nome = var.GetString(1), ExtensaoImagem = var.GetString(2) };
                    categorias.Add(categoria);
                }
                return categorias;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDB.CloseConnection();
            }
        }

        public IList<Categoria> GetCategoriasDesativadas()
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetCategoriasDesativadas]");
            IList<Categoria> categorias = new List<Categoria>();

            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "get_categorias_desativadas";
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlDataReader var = cmd.ExecuteReader();

            try
            {
                while (var.Read())
                {
                    Categoria categoria = new Categoria { IdCategoria = var.GetInt32(0), Nome = var.GetString(1), ExtensaoImagem = var.GetString(2) };
                    categorias.Add(categoria);
                }
                return categorias;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDB.CloseConnection();
            }
        }

        public IList<Produto> GetProdutosCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> GetProdutosCategoria]");

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

        public bool IsAtiva(int idCategoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> isAtiva]");

            _connectionDB.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "is_categoria_ativa";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", idCategoria);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDB.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public int RegistarCategoria(Categoria categoria)
        {
            _logger.LogDebug("A executar [CategoriaDAO -> RegistarCategoria]");

            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "registar_categoria";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", categoria.Nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?extensao_imagem", categoria.ExtensaoImagem);
            cmd.Parameters["?extensao_imagem"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDB.CloseConnection();

            return Convert.ToInt32(val);
        }
    }
}