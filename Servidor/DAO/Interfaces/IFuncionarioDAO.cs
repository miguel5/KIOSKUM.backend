using Entities;

namespace DAO.Interfaces
{
    public interface IFuncionarioDAO
    {
        bool ExisteNumFuncionario(int numFuncionario);
        void EditarConta(Funcionario funcionario);
        Funcionario GetContaNumFuncionario(int numFuncionario);
        void InserirConta(Funcionario funcionario);
    }
}
