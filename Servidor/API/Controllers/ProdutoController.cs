using System;
using API.Business;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }


        /*[HttpPost("add")]
        public IActionResult AddProduto([FromBody] AddProdutoModel model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                bool res = _produtoService.AddProduto(model.Nome, model.NomeCategoria, model.Preco, model.Ingredientes, model.Alergenios);
                if (res == false)
                {
                    return BadRequest(new { message = "Dados Inseridos inválidos" });
                }
                return Ok("Adicionou");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }

        }*/

    }
}
