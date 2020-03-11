using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private List<Produto> produtos;
        private readonly ILogger<ProdutoController> logger;

        public ProdutoController(ILogger<ProdutoController> logger)
        {
            this.logger = logger;
            produtos = new List<Produto>();
            Produto p;
            for (int i = 0; i < 5; i++)
            {
                p = new Produto(i, "Massa com Atum", "Prato", 4.25, new List<string> { "Massa", "Atum" }, new List<string> { "Glúten" });
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
        public void AdicionaProduto(string Nome, string Categoria, double Preco)
        {
            Produto p = new Produto(0, Nome, Categoria, Preco, new List<string> { "Massa", "Atum" }, new List<string> { "Glúten" });
            produtos.Add(p);
            Console.WriteLine(p.ToString());
        }
    }
}
