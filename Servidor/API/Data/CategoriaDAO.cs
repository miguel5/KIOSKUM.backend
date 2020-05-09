using System;
using System.Collections.Generic;
using API.Business;
using API.Entities;

namespace API.Data
{
    public interface ICategoriaDAO
    {
        bool ExisteCategoria(int idCategoria);//determina se o idCategoria ja se encontra no sistema
        bool ExisteNomeCategoria(string nome);//determina se o nome da categoria ja existe no sistema
        Categoria GetCategoriaNome(string nome);//devolve a categoria dando o nome
        bool isAtiva(int idCategoria);//determina se uma categoria esta ou não ativa
        int RegistarCategoria(Categoria categoria);//devolve o id da categoria
        Categoria GetCategoria(int idCategoria);//Retorna uma categoria (ativada/desativada)
        void EditarCategoria(Categoria novaCategoria);//apenas edita se estiver ativada
        IList<Categoria> GetCategoriasDesativadas();//devolve todas as categorias desativada
        IList<Categoria> GetCategorias();//devolve todas as categorias ativadas
        IList<Produto> GetProdutosCategoria(int idCategoria);//devolve todos os produtos ativados de uma categoria
    }

    public class CategoriaDAO : ICategoriaDAO
    {
        private readonly IConnectionDB _connectionDB;

        public CategoriaDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

        public void EditarCategoria(Categoria novaCategoria)
        {
            throw new NotImplementedException();
        }

        public bool ExisteCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public bool ExisteNomeCategoria(string nome)
        {
            throw new NotImplementedException();
        }

        public Categoria GetCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public Categoria GetCategoriaNome(string nome)
        {
            throw new NotImplementedException();
        }

        public IList<Categoria> GetCategorias()
        {
            throw new NotImplementedException();
        }

        public IList<Categoria> GetCategoriasDesativadas()
        {
            throw new NotImplementedException();
        }

        public IList<Produto> GetProdutosCategoria(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public bool isAtiva(int idCategoria)
        {
            throw new NotImplementedException();
        }

        public int RegistarCategoria(Categoria categoria)
        {
            throw new NotImplementedException();
        }
    }
}