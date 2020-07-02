using System.Collections.Generic;
using Entities;

namespace DAO.Interfaces
{
    public interface ICategoriaDAO
    {
        bool ExisteNomeCategoria(string nome);
        Categoria GetCategoriaNome(string nome);
        bool IsAtiva(int idCategoria);
        int RegistarCategoria(Categoria categoria);
        Categoria GetCategoria(int idCategoria);
        void EditarCategoria(Categoria novaCategoria);
        IList<Categoria> GetCategoriasDesativadas();
        IList<Categoria> GetCategoriasAtivadas();
        bool ExisteCategoria(int idCategoria);
        IList<Produto> GetProdutosAtivadosCategoria(int idCategoria);
        IList<Produto> GetProdutosDesativadosCategoria(int idCategoria);
        int GetNumProdutosAtivados(int idCategoria);
        void DesativarCategoria(int idCategoria);
        void AtivarCategoria(int idCategoria);
    }
}
