using System;
using System.Collections.Generic;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    [ApiController]
    [Route("api/produto")]
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
                p = new Produto();
                p.IdProduto = i;
                p.Nome = "Massa com Atum";
                p.Categoria = new Categoria
                {
                    IdCategoria = 1,
                    Nome = "Prato"
                };
                p.Preco = 4.25;
                p.Ingredientes = new List<string> { "Massa", "Atum" };
                p.Alergenios = new List<string> { "Glúten" };
                produtos.Add(p);
            }
        }


        [HttpGet]
        [Route("todos")]
        public IList<Produto> Get()
        {
            return produtos;
        
        }


        [HttpPost]
        public void AdicionaProduto(string Nome, string Categoria, double Preco)
        {
           
        }
    }
}
