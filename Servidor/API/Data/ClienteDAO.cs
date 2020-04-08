using System;
using System.Data;
using API.Entities;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public interface IClienteDAO
    {
        public bool ExisteEmail(string email);
        bool ExisteNumTelemovel(int numTelemovel);
        void InserirCliente(Cliente cliente, string codigoValidacao, int numMaxTentativas);
        string GetCodigoValidacao(string email);
        bool ContaConfirmada(string email);
        Cliente GetClienteEmail(string email);
        Cliente GetClienteId(int idCliente);
        void ValidarConta(string email);
        void EditarConta(Cliente cliente);
        int GetNumTentativas(string email);
    }


    public class ClienteDAO : IClienteDAO
    {
        private readonly IConnectionDB _connectionDB;

        public ClienteDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }


        public bool ExisteEmail(string email)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "existe_email";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?mail", email);
                cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                object val = cmd.ExecuteScalar();
                
                _connectionDB.CloseConnection();
                
                return Convert.ToBoolean(val);
            }
            return false;
        }

        public bool ExisteNumTelemovel(int numTelemovel)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "existe_telemovel";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?telemovel", numTelemovel);
                cmd.Parameters["?telemovel"].Direction = ParameterDirection.Input;

                object val = cmd.ExecuteScalar();
                

                _connectionDB.CloseConnection();

                return (val != null ? Convert.ToBoolean(val) : false);
            }
            return false;
        }

        public void InserirCliente(Cliente cliente, string codigoValidacao, int numMaxTentativas)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "inserir_cliente";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?nome", cliente.Nome);
                cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?password", cliente.Password);
                cmd.Parameters["?password"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?email", cliente.Email);
                cmd.Parameters["?email"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?numTelemovel", cliente.NumTelemovel);
                cmd.Parameters["?numTelemovel"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?codigo_validacao", codigoValidacao);
                cmd.Parameters["?codigo_validacao"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?numMaxTentativas", numMaxTentativas);
                cmd.Parameters["?numMaxTentativas"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

                _connectionDB.CloseConnection();
            }
        }

        public string GetCodigoValidacao(string email)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "get_codigo_validacao";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?mail", email);
                cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                string val = (string)cmd.ExecuteScalar();

                _connectionDB.CloseConnection();

                return val;
            }
            return null;
        }

        public bool ContaConfirmada(string email)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "conta_confirmada";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?mail", email);
                cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                object val = cmd.ExecuteScalar();

                _connectionDB.CloseConnection();

                return (val != null ? Convert.ToBoolean(val) : false);
            }
            return false;
        }

        public int GetNumTentativas(string email)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "num_tentativas";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?mail", email);
                cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                int val = (byte)cmd.ExecuteScalar();

                _connectionDB.CloseConnection();

                return val;
            }
            return -1;
        }


        public void ValidarConta(string email)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "validar_conta";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?mail", email);
                cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

                _connectionDB.CloseConnection();
            }
        }

        public Cliente GetClienteEmail(string email)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "get_cliente_mail";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?mail", email);
                cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                MySqlDataReader var = cmd.ExecuteReader();

                Cliente cliente = null;
                try
                {
                    while (var.Read())
                    {
                        cliente = new Cliente { IdCliente = var.GetInt32(0), Nome = var.GetString(1), Email = email, Password = var.GetString(2), NumTelemovel = var.GetInt32(3) };
                    }
                    return cliente;
                }
                catch (Exception) { }
                finally
                {
                    _connectionDB.CloseConnection();
                }

            }
            return null;
        }

        public Cliente GetClienteId(int idCliente)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "get_cliente_id";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?id", idCliente);
                cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                MySqlDataReader rdr = cmd.ExecuteReader();
                
                try
                {
                    Cliente cliente = null;
                    while (rdr.Read())
                    {
                        cliente = new Cliente { IdCliente = idCliente, Nome = rdr.GetString(0), Password = rdr.GetString(1), Email = rdr.GetString(2), NumTelemovel = rdr.GetInt32(3) };
                    }
                    return cliente;
                }
                catch (Exception) { }
                finally
                {
                    _connectionDB.CloseConnection();
                }
            }
            return null;
        }

        public void EditarConta(Cliente cliente)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "editar_dados";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?id", cliente.IdCliente);
                cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?nome", cliente.Nome);
                cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?password", cliente.Password);
                cmd.Parameters["?password"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?email", cliente.Email);
                cmd.Parameters["?email"].Direction = ParameterDirection.Input;

                cmd.Parameters.AddWithValue("?numTelemovel", cliente.NumTelemovel);
                cmd.Parameters["?numTelemovel"].Direction = ParameterDirection.Input;

                cmd.ExecuteNonQuery();

                _connectionDB.CloseConnection();
            }
        }
    }
}