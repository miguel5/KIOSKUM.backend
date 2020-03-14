using System;
using API.Models;

namespace API.Data
{
    public class CategoriaDAO
    {
        public CategoriaDAO()
        {
        }

        internal bool ContainsNome(string nome)
        {
            throw new NotImplementedException();
        }

        private int GetLastId()
        {
            throw new NotImplementedException();
        }

        internal void AddNovaCategoria(string Nome)
        {
            int idCategoria = GetLastId();
        }
    }
}
