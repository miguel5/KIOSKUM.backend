using System;
using System.Collections.Generic;
using API.Entities;

namespace API.Data
{
    public interface IProdutoDAO
    {
        bool ExisteNome(string nome);
        void AddProduto(Produto produto);
        bool ExisteIdProduto(int IdProduto);
    }

    public class ProdutoDAO : IProdutoDAO
    {
        

        public ProdutoDAO()
        {
        }

        bool IProdutoDAO.ExisteNome(string nome)
        {
            throw new NotImplementedException();
        }

        public void AddProduto(Produto produto)
        {
            throw new NotImplementedException();
        }

        public bool ExisteIdProduto(int IdProduto)
        {
            return true;//throw new NotImplementedException();
        }
    }
}
