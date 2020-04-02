using System;
namespace API.Data
{
    public interface IProdutoDAO
    {

    }

    public class ProdutoDAO : IProdutoDAO
    {
        

        public ProdutoDAO()
        {
        }

        internal bool ExisteNome(string nome)
        {
            throw new NotImplementedException();
        }

        internal void AddProduto(object produto)
        {
            throw new NotImplementedException();
        }
    }
}
