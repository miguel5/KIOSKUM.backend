using API.Entities;

namespace API.Data
{
    public interface IAdministradorDAO
    {
        bool ExisteEmail(string email);
        void InserirAdministrador(Administrador administrador);
        Administrador GetAdministradorEmail(string email);
        bool ExisteNumFuncionario(int numFuncionario);
    }

    public class AdministradorDAO
    {
        public AdministradorDAO()
        {
        }
    }
}
