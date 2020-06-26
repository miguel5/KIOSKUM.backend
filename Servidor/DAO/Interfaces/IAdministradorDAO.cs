using Entities;

namespace DAO.Interfaces
{
    public interface IAdministradorDAO
    {
        bool ExisteNumFuncionario(int numFuncionario);
        void InserirConta(Administrador administrador);
        Administrador GetContaNumFuncionario(int numFuncionario);
        Administrador GetContaIdFuncionario(int idFuncionario);
        void EditarConta(Administrador administradorEditado);
    }
}