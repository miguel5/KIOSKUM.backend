using System.Collections.Generic;
using API.Entities;

namespace API.Data.Interfaces
{
    public interface IProdutoDAO
    {
        bool ExisteNomeProduto(string nome);
        int RegistarProduto(Produto produto);//retorna o idProduto
        Produto GetProduto(int idProduto);//devolve o produto (ativado/desativado)
        Produto GetProdutoNome(string nome);//devolve o produto (ativado/desativado)
        void EditarProduto(Produto produto);
        IList<Produto> GetProdutosDesativados();//apenas devolve os desativados
        void DesativarProduto(int idProduto);
        void AtivarProduto(int idProduto);
        bool IsAtivo(int idProduto);
        bool ExisteProduto(int idProduto);
    }
}
