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
        private List<Produto> produtos = new List<Produto>
        {
            new Produto("Massa com Atum", "Prato", 4.25, new List<string> { "Massa", "Atum" }, new List<string> { "Glúten" }, "/azure/Massa_com_Atum.jpeg")
        };
        private readonly ILogger<ProdutoController> logger;

        public ProdutoController(ILogger<ProdutoController> logger)
        {
            this.logger = logger;
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
