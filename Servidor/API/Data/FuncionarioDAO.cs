using System;
using API.Business;
using API.Data.Interfaces;
using API.Entities;
using API.Services;

namespace API.Data
{
    public class FuncionarioDAO : IFuncionarioDAO
    {
        private readonly IConnectionDBService _connectionDB;


        public FuncionarioDAO(IConnectionDBService connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void EditarNome(string nome)
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
