using System;
using System.Data;
using DAO.Interfaces;
using Entities;
using Services.DBConnection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DAO
{
    public class AdministradorDAO : IAdministradorDAO
    {
        private readonly ILogger _logger;
        private readonly IConnectionDBService _connectionDBService;

        public AdministradorDAO(ILogger<AdministradorDAO> logger, IConnectionDBService connectionDBService)
        {
            _logger = logger;
            _connectionDBService = connectionDBService;
        }

        public void EditarConta(Administrador administrador)
        {
            _logger.LogDebug("A executar [AdministradorDAO -> EditarConta]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "editar_conta_admin";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", administrador.IdFuncionario);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?nome", administrador.Nome);
                    cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?numero", administrador.NumFuncionario);
                    cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?mail", administrador.Email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?pass", administrador.Password);
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

        public bool ExisteEmail(string email)
        {
            _logger.LogDebug("A executar [AdministradorDAO -> ExisteEmail]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_mail_admin";
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

        public bool ExisteNumFuncionario(int numFuncionario)
        {
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

        public Administrador GetContaEmail(string email)
        {
            _logger.LogDebug("A executar [AdministradorDAO -> GetContaEmail]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_admin_mail";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?mail", email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {

                        Administrador administrador = null;
                        if (var.Read())
                        {
                            administrador = new Administrador { IdFuncionario = var.GetInt32(0), Nome = var.GetString(1), NumFuncionario = var.GetInt32(2), Email = email, Password = var.GetString(3) };
                        }

                        return administrador;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Administrador GetContaId(int idFuncionario)
        {
            _logger.LogDebug("A executar [AdministradorDAO -> GetContaId]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_admin_id";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idFuncionario);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {

                        Administrador administrador = null;

                        if (var.Read())
                        {
                            administrador = new Administrador
                            {
                                IdFuncionario = idFuncionario,
                                Nome = var.GetString(0),
                                NumFuncionario = var.GetInt32(1),
                                Email = var.GetString(2),
                                Password = var.GetString(3)
                            };
                        }
                        return administrador;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public void InserirConta(Administrador administrador)
        {
            _logger.LogDebug("A executar [AdministradorDAO -> InserirConta]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "inserir_admin";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?nome", administrador.Nome);
                    cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?numero", administrador.NumFuncionario);
                    cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?mail", administrador.Email);
                    cmd.Parameters["?mail"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?pass", administrador.Password);
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