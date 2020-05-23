using System;
using System.Collections.Generic;
using API.Entities;

namespace API.Data.Interfaces
{
    public interface ICategoriaDAO
    {
        bool ExisteCategoria(int idCategoria);//determina se o idCategoria ja se encontra no sistema
        bool ExisteNomeCategoria(string nome);//determina se o nome da categoria ja existe no sistema
        Categoria GetCategoriaNome(string nome);//devolve a categoria dando o nome
        bool IsAtiva(int idCategoria);//determina se uma categoria esta ou não ativa
        int RegistarCategoria(Categoria categoria);//devolve o id da categoria
        Categoria GetCategoria(int idCategoria);//Retorna uma categoria (ativada/desativada)
        void EditarCategoria(Categoria novaCategoria);//apenas edita se estiver ativada
        IList<Categoria> GetCategoriasDesativadas();//devolve todas as categorias desativada
        IList<Categoria> GetCategorias();//devolve todas as categorias ativadas
        IList<Produto> GetProdutosCategoria(int idCategoria);//devolve todos os produtos ativados de uma categoria
    }
}
