using System;

namespace API.Data
{
    public interface ICategoriaDAO
    {
        int GetIdCategoria(string nomeCategoria);
    }

    public class CategoriaDAO : ICategoriaDAO
    {
        public CategoriaDAO()
        {
        }

        public int GetIdCategoria(string nomeCategoria)
        {
            throw new NotImplementedException();
        }
    }
}
