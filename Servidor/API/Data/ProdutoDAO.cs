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
        IList<Produto> GetProdutosCategoria(int idCategoria);//apenas devolve os ativados
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
                        produto = new Produto { IdProduto = idProduto, Nome = var.GetString(0), IdCategoria = var.GetInt32(3) , Preco = var.GetDecimal(1), Ingredientes = new List(), Alergenicos = new List(), ExtensaoImagem = var.GetString(2) };

                        cmd.CommandText = "get_ingredientes_produto";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                        cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                        var = cmd.ExecuteReader();

                        while (var.Read())
                        {
                            Produto.Ingrediente.Add(var.getString(0));
                        }

                        cmd.CommandText = "get_alergenicos_produto";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                        cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                        var = cmd.ExecuteReader();

                        while (var.Read())
                        {
                            Produto.Alergenicos.Add(var.getString(0));
                        }
                    }
                    return produto;
                }
                catch (Exception) { }
                finally
                {
                    _connectionDB.CloseConnection();
                }

            }
            return null;
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
                        produto = new Produto { IdProduto = var.GetInt32(0), Nome = nome, IdCategoria = var.getInt32(3) , Preco = var.getDecimal(1), Ingredientes = new List(), Alergenicos = new List(), ExtensaoImagem = var.GetString(2) };

                        cmd.CommandText = "get_ingredientes_produto";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                        cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                        var = cmd.ExecuteReader();

                        while (var.Read())
                        {
                            Produto.Ingrediente.Add(var.getString(0));
                        }

                        cmd.CommandText = "get_alergenicos_produto";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                        cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                        var = cmd.ExecuteReader();

                        while (var.Read())
                        {
                            Produto.Alergenicos.Add(var.getString(0));
                        }
                    }
                    return produto;
                }
                catch (Exception) { }
                finally
                {
                    _connectionDB.CloseConnection();
                }

            }
            return null;
        }

        public IList<Produto> GetProdutosCategotia(int idCategoria)
        {
            IList<Produto> produtos = new List<>();

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
                    Produto produto = new Produto { IdProduto = var.GetInt32(0), Nome = car.GetString(1), IdCategoria = idCategoria , Preco = var.getDecimal(2), Ingredientes = new List(), Alergenicos = new List(), ExtensaoImagem = var.GetString(3) };

                    cmd.CommandText = "get_ingredientes_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        Produto.Ingrediente.Add(var.getString(0));
                    }

                    cmd.CommandText = "get_alergenicos_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        Produto.Alergenicos.Add(var.getString(0));
                    }
                    
                    produtos.Add(produto);
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

     public IList<Produto> GetProdutosDesativados()
        {
            IList<Produto> produtos = new List<>();

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
                    Produto produto = new Produto { IdProduto = var.GetInt32(0), Nome = car.GetString(1), IdCategoria = idCategoria , Preco = var.getDecimal(2), Ingredientes = new List(), Alergenicos = new List(), ExtensaoImagem = var.GetString(3) };

                    cmd.CommandText = "get_ingredientes_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        Produto.Ingrediente.Add(var.getString(0));
                    }

                    cmd.CommandText = "get_alergenicos_produto";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                    cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                    var = cmd.ExecuteReader();

                    while (var.Read())
                    {
                        Produto.Alergenicos.Add(var.getString(0));
                    }
                    
                    produtos.Add(produto);
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

            int productId = cmd.ExecuteScalar();

            foreach(string ingrediente in Produto.Ingredientes) {
                cmd.CommandText = "adicionar_ingrediente";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?nome", ingrediente);
                cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                int ingredientId = cmd.ExecuteScalar();

                cmd.CommandText = "adicionar_produto_ingrediente";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?produto", productId);
                cmd.Parameters["?produto"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?ingrediente", ingredientId);
                cmd.Parameters["?ingrediente"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuerry();

            }

            foreach(string alergenico in Produto.Alergenicos) {
                cmd.CommandText = "adicionar_alergenico";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?nome", alergenico);
                cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                int alergenicId = cmd.ExecuteScalar();

                cmd.CommandText = "adicionar_produto_alergenico";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?produto", productId);
                cmd.Parameters["?produto"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?alergenico", alergenicId);
                cmd.Parameters["?alergenico"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuerry();

            }

            _connectionDB.CloseConnection();
        }
    }
}

