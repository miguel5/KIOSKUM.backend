using System.Text;

namespace API.ViewModel
{
    public class CategoriaModelView
    {
        public int IdCategoria { get; set;  }
        public string Nome { get; set; }
        public string URLProduto { get; set; }

        public CategoriaModelView() { }

        public CategoriaModelView(int IdCategoria, string Nome, string URLProduto)
        {
            this.IdCategoria = IdCategoria;
            this.Nome = Nome;
            this.URLProduto = URLProduto;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Produto\n");
            sb.Append("- ID : " + IdCategoria + "\n");
            sb.Append("- Nome : " + Nome + "\n");
            return sb.ToString();
        }
    }
}
