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
        void InserirCliente(Cliente cliente, string codigoValidacao);
        string GetCodigoValidacao(string email);
        bool ContaValida(string email);
        void ValidarConta(string email);
        void EditarConta(Cliente cliente);
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
                
                return (val != null ? Convert.ToBoolean(val) : false);
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

        public void InserirCliente(Cliente cliente, string codigoValidacao)
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

        public bool ContaValida(string email)
        {
            MySqlCommand cmd;
            if (_connectionDB.OpenConnection())
            {
                Console.WriteLine(email);
                cmd = new MySqlCommand();
                cmd.Connection = _connectionDB.Connection;

                cmd.CommandText = "conta_valida";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("?mail", email);
                cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                object val = cmd.ExecuteScalar();

                _connectionDB.CloseConnection();

                return (val != null ? Convert.ToBoolean(val) : false);
            }
            return false;
        }

        internal int GetNumTentativas(string email)
        {
            throw new NotImplementedException();
        }

        internal void IncrementaNumTentativas(string email)
        {
            throw new NotImplementedException();
        }

        internal void RemoverContaInvalida(string email)
        {
            throw new NotImplementedException();
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

        internal Cliente GetClienteEmail(string email)
        {
            throw new NotImplementedException();
        }

        internal Cliente GetClienteId(int idCliente)
        {
            throw new NotImplementedException();
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