using System;
using API.Data.Interfaces;
using API.Entities;
using API.Services.DBConnection;

namespace API.Data
{
    public class FuncionarioDAO : IFuncionarioDAO
    {
        private readonly  IConnectionDBService _connectionDBService;


        public FuncionarioDAO(IConnectionDBService connectionDBService)
        {
            _connectionDBService = connectionDBService;
        }

        public void EditarConta(Funcionario funcionario)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "editar_conta_funcionario";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?id", funcionario.IdFuncionario);
            cmd.Parameters["?id"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?nome", funcionario.Nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?numero", funcionario.NumFuncionario);
            cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDBService.CloseConnection();
        }

        public bool ExisteNumFuncionario(int numFuncionario)
        {
            _connectionDBService.OpenConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "existe_num_funcionario";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?numero", numFuncionario);
            cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

            object val = cmd.ExecuteScalar();

            _connectionDBService.CloseConnection();

            return Convert.ToBoolean(val);
        }

        public Funcionario GetContaNumFuncionario(int numFuncionario)
        {
            _connectionDBService.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "get_funcionario_numero";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?numero", numFuncionario);
            cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

            MySqlDataReader var = cmd.ExecuteReader();

            Funcionario funcionario = null;
            try
            {
                if (var.Read())
                {
                    funcionario = new Funcionario { IdFuncionario = var.GetInt32(0), Nome = var.GetString(1), NumFuncionario = numFuncionario };
                }
                return funcionario;
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public void InserirConta(Funcionario funcionario)
        {
            _connectionDBService.OpenConnection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = _connectionDBService.Connection;

            cmd.CommandText = "inserir_funcionario";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("?nome", funcionario.Nome);
            cmd.Parameters["?nome"].Direction = ParameterDirection.Input;

            cmd.Parameters.AddWithValue("?numero", funcionario.NumFuncionario);
            cmd.Parameters["?numero"].Direction = ParameterDirection.Input;

            cmd.ExecuteNonQuery();

            _connectionDBService.CloseConnection();
        }
    }
}