using API.Entities;

namespace API.Data.Interfaces
{
    public interface IFuncionarioDAO
    {
        bool ExisteNumFuncionario(int numFuncionario);
        void EditarNome(string nome);
        Funcionario GetContaNumFuncionario(int numFuncionario);
        void InserirConta(Funcionario funcionario);
    }
}
