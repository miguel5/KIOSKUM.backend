using System;
using System.Data;
using DAO.Interfaces;
using Entities;
using Services.DBConnection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DAO
{
    public class FuncionarioDAO : IFuncionarioDAO
    {
        private readonly ILogger _logger;
        private readonly IConnectionDBService _connectionDBService;


        public FuncionarioDAO(ILogger<FuncionarioDAO> logger, IConnectionDBService connectionDBService)
        {
            _logger = logger;
            _connectionDBService = connectionDBService;
        }

        public void EditarConta(Funcionario funcionario)
        {
            _logger.LogDebug("A executar [FuncionarioDAO -> EditarConta]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "editar_conta_funcionario";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", funcionario.IdFuncionario);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?nome", funcionario.Nome);
                    cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?numero", funcionario.NumFuncionario);
                    cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?pass", funcionario.Password);
                    cmd.Parameters["?pass"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public bool ExisteIdFuncionario(int idFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioDAO -> ExisteIdFuncionario]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_id_funcionario";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idFuncionario);
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

        public bool ExisteNumFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioDAO -> ExisteNumFuncionario]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_num_funcionario";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?numero", numFuncionario);
                    cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

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

        public Funcionario GetContaIdFuncionario(int idFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioDAO -> GetContaIdFuncionario]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_funcionario_id";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idFuncionario);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {

                        Funcionario funcionario = null;

                        if (var.Read())
                        {
                            funcionario = new Funcionario { IdFuncionario = idFuncionario, Nome = var.GetString(0), NumFuncionario = var.GetInt32(1), Password = var.GetString(1) };
                        }
                        return funcionario;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Funcionario GetContaNumFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioDAO -> GetContaNumFuncionario]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_funcionario_numero";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?numero", numFuncionario);
                    cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {

                        Funcionario funcionario = null;

                        if (var.Read())
                        {
                            funcionario = new Funcionario { IdFuncionario = var.GetInt32(0), Nome = var.GetString(1), NumFuncionario = numFuncionario, Password = var.GetString(2) };
                        }
                        return funcionario;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public void InserirConta(Funcionario funcionario)
        {
            _logger.LogDebug("A executar [FuncionarioDAO -> InserirConta]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "inserir_funcionario";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?nome", funcionario.Nome);
                    cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?numero", funcionario.NumFuncionario);
                    cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?pass", funcionario.Password);
                    cmd.Parameters["?pass"].Direction = ParameterDirection.Input;

                    cmd.ExecuteNonQuery();
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