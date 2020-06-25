using Entities;

namespace DAO.Interfaces
{
    public interface IClienteDAO
    {
        public bool ExisteEmail(string email);
        bool ExisteNumTelemovel(int numTelemovel);
        void InserirConta(Cliente cliente, string codigoValidacao, int numMaxTentativas);
        string GetCodigoValidacao(string email);
        void DecrementaTentativas(string email);
        bool ContaConfirmada(string email);
        Cliente GetContaEmail(string email);
        Cliente GetContaId(int idCliente);
        void ValidarConta(string email);
        void EditarConta(Cliente cliente);
        int GetNumTentativas(string email);
        bool ExisteCliente(int idCliente);
    }
}
