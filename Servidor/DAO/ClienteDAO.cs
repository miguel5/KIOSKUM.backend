using System;
using System.Data;
using DAO.Interfaces;
using Entities;
using Services.DBConnection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DAO
{
    public class ClienteDAO : IClienteDAO
    {
        private readonly ILogger _logger;
        private readonly IConnectionDBService _connectionDBService;

        public ClienteDAO(ILogger<ClienteDAO> logger, IConnectionDBService connectionDBService)
        {
            _logger = logger;
            _connectionDBService = connectionDBService;
        }


        public bool ExisteEmail(string email)
        {
            _logger.LogDebug("A executar [ClienteDAO -> ExisteEmail]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_email";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

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

        public bool ExisteNumTelemovel(int numTelemovel)
        {
            _logger.LogDebug("A executar [ClienteDAO -> ExisteNumTelemovel]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_telemovel";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?telemovel", numTelemovel);
                    cmd.Parameters["?telemovel"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return (val != null ? Convert.ToBoolean(val) : false);
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public void InserirConta(Cliente cliente, string codigoValidacao, int numMaxTentativas)
        {
            _logger.LogDebug("A executar [ClienteDAO -> InserirConta]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

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

                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public string GetCodigoValidacao(string email)
        {
            _logger.LogDebug("A executar [ClienteDAO -> GetCodigoValidacao]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_codigo_validacao";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    string val = (string)cmd.ExecuteScalar();

                    return val;
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public bool ContaConfirmada(string email)
        {
            _logger.LogDebug("A executar [ClienteDAO -> ContaConfirmada]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "conta_confirmada";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return (val != null ? Convert.ToBoolean(val) : false);
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public int GetNumTentativas(string email)
        {
            _logger.LogDebug("A executar [ClienteDAO -> GetNumTentativas]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "num_tentativas";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    int val = (byte)cmd.ExecuteScalar();

                    return val;
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }


        public void DecrementaTentativas(string email)
        {
            _logger.LogDebug("A executar [ClienteDAO -> DecrementaTentativas]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "decrementa_tentativas";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }


        public void ValidarConta(string email)
        {
            _logger.LogDebug("A executar [ClienteDAO -> ValidarConta]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "validar_conta";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Cliente GetContaEmail(string email)
        {
            _logger.LogDebug("A executar [ClienteDAO -> GetContaEmail]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_cliente_mail";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    MySqlDataReader var = cmd.ExecuteReader();

                    Cliente cliente = null;
                    while (var.Read())
                    {
                        cliente = new Cliente { IdCliente = var.GetInt32(0), Nome = var.GetString(1), Email = email, Password = var.GetString(2), NumTelemovel = var.GetInt32(3) };
                    }
                    return cliente;
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Cliente GetContaId(int idCliente)
        {
            _logger.LogDebug("A executar [ClienteDAO -> GetContaId]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_cliente_id";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idCliente);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {

                        Cliente cliente = null;
                        while (rdr.Read())
                        {
                            cliente = new Cliente { IdCliente = idCliente, Nome = rdr.GetString(0), Password = rdr.GetString(1), Email = rdr.GetString(2), NumTelemovel = rdr.GetInt32(3) };
                        }
                        return cliente;
                    }
                }
            }
            catch (Exception) { throw;}
            finally
            { 
                _connectionDBService.CloseConnection();
            }
        }

        public void EditarConta(Cliente cliente)
        {
            _logger.LogDebug("A executar [ClienteDAO -> EditarConta]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

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
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public bool ExisteCliente(int idCliente)
        {
            _logger.LogDebug("A executar [ClienteDAO -> ExisteCliente]");

            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_cliente";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idCliente);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return Convert.ToBoolean(val);
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }
    }
}