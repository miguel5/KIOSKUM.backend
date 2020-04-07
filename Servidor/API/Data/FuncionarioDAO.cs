using System;
using API.Entities;

namespace API.Data
{
    public interface IFuncionarioDAO
    {
        bool ExisteNumFuncionario(int numFuncionario);
        void InserirFuncionario(Funcionario funcionario);
    }

    public class FuncionarioDAO
    {
        private readonly IConnectionDB _connectionDB;


        public FuncionarioDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }
    }
}
