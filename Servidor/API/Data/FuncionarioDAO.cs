using System;
using API.Business;
using API.Entities;

namespace API.Data
{
    public interface IFuncionarioDAO
    {
        bool ExisteNumFuncionario(int numFuncionario);
        void EditarNome(string nome);
        Funcionario GetContaNumFuncionario(int numFuncionario);
        void InserirConta(Funcionario funcionario);
    }

    public class FuncionarioDAO : IFuncionarioDAO
    {
        private readonly IConnectionDB _connectionDB;


        public FuncionarioDAO(IConnectionDB connectionDB)
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
