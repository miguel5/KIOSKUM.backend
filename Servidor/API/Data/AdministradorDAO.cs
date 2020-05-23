using API.Data.Interfaces;
using API.Entities;
using API.Services;

namespace API.Data
{
    public class AdministradorDAO : IAdministradorDAO
    {
        private readonly IConnectionDBService _connectionDB;

        public AdministradorDAO(IConnectionDBService connectionDB)
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
