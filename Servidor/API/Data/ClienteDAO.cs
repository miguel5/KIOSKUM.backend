using System;
using System.Data;
using API.Entities;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public interface IClienteDAO
    {
        void InserirCliente(Cliente cliente, string codigoValidacao);
    }


    public class ClienteDAO : IClienteDAO
    {
        private readonly IDBConnection _dbConnection;

        public ClienteDAO(IDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        internal bool ExisteEmail(string email)
        {
            throw new NotImplementedException();
        }

        internal bool ExisteNumTelemovel(int numTelemovel)
        {
            throw new NotImplementedException();
        }

        public void InserirCliente(Cliente cliente, string codigoValidacao)
        {
            MySqlCommand cmd;
            if (_dbConnection.OpenConnection())
            {
                Console.WriteLine(cliente.Nome + "\n" + codigoValidacao);
                cmd = new MySqlCommand();
                cmd.Connection = _dbConnection.Connection;

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

                _dbConnection.CloseConnection();
            }
        }

        internal string GetCodigoValidacao(string email)
        {
            throw new NotImplementedException();
        }

        internal bool ContaValida(string email)
        {
            throw new NotImplementedException();
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

        internal void ValidarConta(string email)
        {
            throw new NotImplementedException();
        }

        internal Cliente GetClienteEmail(string email)
        {
            throw new NotImplementedException();
        }

        internal Cliente GetClienteId(int idCliente)
        {
            throw new NotImplementedException();
        }

        internal void EditarConta(Cliente cliente)
        {
            throw new NotImplementedException();
        }
    }
}
