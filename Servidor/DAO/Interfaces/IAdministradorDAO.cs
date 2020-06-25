using Entities;

namespace DAO.Interfaces
{
    public interface IAdministradorDAO
    {
        bool ExisteEmail(string email);
        void InserirConta(Administrador administrador);
        Administrador GetContaEmail(string email);
        bool ExisteNumFuncionario(int numFuncionario);
        Administrador GetContaId(int idFuncionario);
        void EditarConta(Administrador administrador);
    }
}
