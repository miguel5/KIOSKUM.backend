using API.Business;
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
        void EditarConta(Administrador administrador);
    }

    public class AdministradorDAO : IAdministradorDAO
    {
        private readonly IConnectionDB _connectionDB;

        public AdministradorDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void EditarConta(Administrador administrador)
        {
            throw new System.NotImplementedException();
        }

        public bool ExisteEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        public bool ExisteNumFuncionario(int numFuncionario)
        {
            throw new System.NotImplementedException();
        }

        public Administrador GetContaEmail(string email)
        {
            throw new System.NotImplementedException();
        }

        public Administrador GetContaId(int idFuncionario)
        {
            throw new System.NotImplementedException();
        }

        public void InserirConta(Administrador administrador)
        {
            throw new System.NotImplementedException();
        }
    }
}
