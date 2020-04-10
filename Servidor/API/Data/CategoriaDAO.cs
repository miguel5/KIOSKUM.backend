using System;
using System.Collections.Generic;
using API.Business;
using API.Entities;

namespace API.Data
{
    public interface ICategoriaDAO
    {
        Categoria GetCategoriaNome(string nome);
        void EditarCategoria(Categoria c);
        void AddCategoria(Categoria categoria);
        bool ExisteNomeCategoria(string nome);
        int GetIdCategoria(string nomeCategoria);
        IList<Categoria> GetTodasCategorias();
    }

    public class CategoriaDAO : ICategoriaDAO
    {
        private readonly IConnectionDB _connectionDB;

        public CategoriaDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void AddCategoria(Categoria categoria)
        {
            throw new NotImplementedException();
        }

        public void EditarCategoria(Categoria c)
        {
            throw new NotImplementedException();
        }

        public bool ExisteNomeCategoria(string nome)
        {
            throw new NotImplementedException();
        }

        public Categoria GetCategoriaNome(string nome)
        {
            throw new NotImplementedException();
        }

        public int GetIdCategoria(string nomeCategoria)
        {
            throw new NotImplementedException();
        }

        public IList<Categoria> GetTodasCategorias()
        {
            throw new NotImplementedException();
        }
    }
}
