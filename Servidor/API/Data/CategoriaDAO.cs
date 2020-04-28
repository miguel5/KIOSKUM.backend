using System;
using System.Collections.Generic;
using API.Business;
using API.Entities;

namespace API.Data
{
    public interface ICategoriaDAO
    {
        Categoria GetCategoriaNome(string nome);
        void EditarCategoria(Categoria categoria);
        void AddCategoria(Categoria categoria);
        bool ExisteNomeCategoria(string nome);
        int GetIdCategoria(string nomeCategoria);
        IList<Categoria> GetTodasCategorias();
        bool CategoriaIsEmpty(string nome);
        void RemoverCategoria(string nome);
        string GetNomeCategoria(int idCategoria);
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

        public bool CategoriaIsEmpty(string nome)
        {
            return true;//throw new NotImplementedException();
        }

        public void EditarCategoria(Categoria categoria)
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
            return 1;
        }

        public string GetNomeCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public IList<Categoria> GetTodasCategorias()
        {
            throw new NotImplementedException();
        }

        public void RemoverCategoria(string nome)
        {
            throw new NotImplementedException();
        }
    }
}
