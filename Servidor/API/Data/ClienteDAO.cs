using System;
using API.Entities;

namespace API.Data
{
    public class ClienteDAO
    {
        public ClienteDAO()
        {
        }

        internal void InserirCliente(Cliente cliente, string codigoValidacao)
        {
            throw new NotImplementedException();
        }

        internal bool ExisteEmail(string email)
        {
            throw new NotImplementedException();
        }

        internal bool ExisteNumTelemovel(int numTelemovel)
        {
            throw new NotImplementedException();
        }

        internal Cliente GetClienteEmail(string email)
        {
            throw new NotImplementedException();
        }

        internal Cliente GetClienteId(int idCliente)
        {
            throw new NotImplementedException();
        }

        internal void EditarDados(Cliente cliente)
        {
            throw new NotImplementedException();
        }

        internal object GetCodigoValidacao(string email)
        {
            throw new NotImplementedException();
        }

        internal bool ContaValida(string email)
        {
            throw new NotImplementedException();
        }

        internal void ValidarConta(string email)
        {
            throw new NotImplementedException();
        }
    }
}
