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
            throw new NotImplementedException();
        }

        public bool ExisteNumFuncionario(int numFuncionario)
        {
            throw new NotImplementedException();
        }

        public Funcionario GetContaNumFuncionario(int numFuncionario)
        {
            throw new NotImplementedException();
        }

        public void InserirConta(Funcionario funcionario)
        {
            throw new NotImplementedException();
        }
    }
}
