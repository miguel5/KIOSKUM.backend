using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    [ApiController]
    [Route("Produto")]
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
                p = new Produto("Alonso", "Presunto", 234234.12);
                produtos.Add(p);
            }
        }


        [HttpGet]
        [Route("Produto/Todos")]
        public IList<Produto> Get()
        {
            return produtos;
        }


        [HttpPost]
        public void Escreve(string nome)
        {
            Produto p = new Produto(nome, "Presunto", 234234.12);
            produtos.Add(p);
            Console.WriteLine(p.ToString());
        }
    }
}
