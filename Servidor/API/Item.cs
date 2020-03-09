using System.Text;

namespace API
{
    public class Item
    {
        public int IdProduto { get; }
        public int Quantidade { get; }
        public string Observacoes { get; }

        public Item(int IdProduto, int Quantidade, string Observacoes)
        {
            this.IdProduto = IdProduto;
            this.Quantidade = Quantidade;
            this.Observacoes = Observacoes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Item\n");
            sb.Append("Id Produto : " + IdProduto + "\n");
            sb.Append("- Email: " + Email + "\n");
            sb.Append("Numero de Funcionario: " + NumFuncionario + "\n");
            return sb.ToString();
        }
    }
}