using System.Collections.Generic;
using Entities;

namespace DAO.Interfaces
{
    public interface IProdutoDAO
    {
        bool ExisteNomeProduto(string nome);
        int RegistarProduto(Produto produto);//retorna o idProduto
        Produto GetProduto(int idProduto);//devolve o produto (ativado/desativado)
        Produto GetProdutoNome(string nome);//devolve o produto (ativado/desativado)
        void EditarProduto(Produto produto);
        void DesativarProduto(int idProduto);
        void AtivarProduto(int idProduto);
        bool IsAtivo(int idProduto);
        bool ExisteProduto(int idProduto);
    }
}
