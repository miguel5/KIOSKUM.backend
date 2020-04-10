using API.Entities;

namespace API.Data
{
    public interface IAdministradorDAO
    {
        bool ExisteEmail(string email);
        void InserirConta(Administrador administrador);
        Administrador GetContaEmail(string email);
        bool ExisteNumFuncionario(int numFuncionario);
        Administrador GetContaId(int idFuncionario);
        void EditarConta(Administrador a);
    }

    public class AdministradorDAO : IAdministradorDAO
    {
        private readonly IConnectionDB _connectionDB;

        public AdministradorDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        void IAdministradorDAO.EditarConta(Administrador a)
        {
            throw new System.NotImplementedException();
        }

        bool IAdministradorDAO.ExisteEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        bool IAdministradorDAO.ExisteNumFuncionario(int numFuncionario)
        {
            throw new System.NotImplementedException();
        }

        Administrador IAdministradorDAO.GetContaEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        Administrador IAdministradorDAO.GetContaId(int idFuncionario)
        {
            throw new System.NotImplementedException();
        }

        void IAdministradorDAO.InserirConta(Administrador administrador)
        {
            throw new System.NotImplementedException();
        }
    }
}
