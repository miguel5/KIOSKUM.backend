using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private List<Produto> produtos;

        private readonly ILogger<ProdutoController> _logger;

        public ProdutoController(ILogger<ProdutoController> logger)
        {
            _logger = logger;
            produtos = new List<Produto>();
            Produto p;
            for (int i = 0; i < 5; i++)
            {
                p = new Produto("Massa com Atum", "Prato", 4.25, new List<string> { "Massa", "Atum" }, new List<string> { "Glúten" }, "/azure/Massa_com_Atum.jpeg");
                produtos.Add(p);
            }
        }


        [HttpGet]
        [Route("Todos")]
        public IList<Produto> Get()
        {
            return produtos;
        
        }


        [HttpPost]
        public void AdicionaProduto(string Nome, string Categoria, double Preco, string PathImagem)
        {
            Produto p = new Produto(Nome, Categoria, Preco, new List<string> { "Massa", "Atum" }, new List<string> { "Glúten" }, PathImagem);
            produtos.Add(p);
            Console.WriteLine(p.ToString());
        }
    }
}
