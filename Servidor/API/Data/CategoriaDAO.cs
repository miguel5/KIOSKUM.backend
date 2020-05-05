using System;
using API.Business;

namespace API.Data
{
    public interface ICategoriaDAO
    {
        bool ExisteCategoria(int idCategoria);
    }

    public class CategoriaDAO : ICategoriaDAO
    {
        private readonly IConnectionDB _connectionDB;

        public CategoriaDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public bool ExisteCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }
    }
}