using System;
using System.Collections.Generic;
using API.Business;
using API.Entities;

namespace API.Data
{
    public interface ICategoriaDAO
    {
        bool ExisteCategoria(int idCategoria);//determina se o idCategoria ja se encontra no sistema
        bool ExisteNomeCategoria(string nome);//determina se o nome da categoria ja existe no sistema
        Categoria GetCategoriaNome(string nome);//devolve a categoria dando o nome
        bool isAtiva(int idCategoria);//determina se uma categoria esta ou não ativa
        int RegistarCategoria(Categoria categoria);//devolve o id da categoria
        Categoria GetCategoria(int idCategoria);//Retorna uma categoria (ativada/desativada)
        void EditarCategoria(Categoria novaCategoria);//apenas edita se estiver ativada
        IList<Categoria> GetCategoriasDesativadas();//devolve todas as categorias desativada
        IList<Categoria> GetCategorias();//devolve todas as categorias ativadas
        IList<Produto> GetProdutosCategoria(int idCategoria);//devolve todos os produtos ativados de uma categoria
    }

    public class CategoriaDAO : ICategoriaDAO
    {
        private readonly IConnectionDB _connectionDB;

        public CategoriaDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void EditarCategoria(Categoria novaCategoria)
        {
            _connectionDB.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDB.Connection;

            cmd.CommandText = "editar_categoria";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", produto.IdProduto);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?nome", produto.Nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?extensao_imagem", produto.ExtensaoImagem);
            cmd.Parameters["?extensao_imagem"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDB.CloseConnection();
        }

        public bool ExisteCategoria(int idCategoria)
        {
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
                catch (Exception) { }
                finally
                {
                    _connectionDB.CloseConnection();
                }

            }
            return null;
        }

        public Categoria GetCategoriaNome(string nome)
        {
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
                catch (Exception) { }
                finally
                {
                    _connectionDB.CloseConnection();
                }

            }
            return null;
        }

        public IList<Categoria> GetCategorias()
        {
            IList<Categoria> categorias = new List<>();

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
            }
            catch (Exception) { }
            finally
            {
                _connectionDB.CloseConnection();
            }

        }
        return null;
    }

        public IList<Categoria> GetCategoriasDesativadas()
        {
            IList<Categoria> categorias = new List<>();

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
            }
            catch (Exception) { }
            finally
            {
                _connectionDB.CloseConnection();
            }

        }
        return null;
    }

        public bool isAtiva(int idCategoria)
        {
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

        public int RegistarCategoria(Categoria categoria)
        {
            throw new NotImplementedException();
        }
    }
}