using System;
using API.Entities;

namespace API.Data
{
    public interface IFuncionarioDAO
    {
        bool ExisteNumFuncionario(int numFuncionario);
        void EditarNomeFuncionario(string nome);
        Funcionario GetFuncionario(int numFuncionario);
        void InserirFuncionario(Funcionario funcionario);
    }

    public class FuncionarioDAO : IFuncionarioDAO
    {
        private readonly IConnectionDB _connectionDB;


        public FuncionarioDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void EditarNomeFuncionario(string nome)
        {
            throw new NotImplementedException();
        }

        public bool ExisteNumFuncionario(int numFuncionario)
        {
            throw new NotImplementedException();
        }

        public Funcionario GetFuncionario(int numFuncionario)
        {
            throw new NotImplementedException();
        }

        public void InserirFuncionario(Funcionario funcionario)
        {
            throw new NotImplementedException();
        }
    }
}
