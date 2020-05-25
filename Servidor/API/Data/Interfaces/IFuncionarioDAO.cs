using API.Entities;

namespace API.Data.Interfaces
{
    public interface IFuncionarioDAO
    {
        bool ExisteNumFuncionario(int numFuncionario);
        void EditarConta(Funcionario funcionario);
        Funcionario GetContaNumFuncionario(int numFuncionario);
        void InserirConta(Funcionario funcionario);
    }
}
