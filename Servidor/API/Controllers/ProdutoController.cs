using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Business;
using API.Models;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Administrador,Cliente")]
    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }


        [HttpPost("add")]
        public IActionResult AddProduto([FromBody] ProdutoDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                IList<int> erros = _produtoService.AddProduto(model.Nome, model.NomeCategoria, model.Preco, model.Ingredientes, model.Alergenios);
                if (erros.Any())
                {
                    return BadRequest(new ErrosDTO { Erros = erros });
                }
                return Ok();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }

        }




        [AllowAnonymous]
        [HttpPost("upload/imagem")]
        public async Task<IActionResult> UploadImagem([FromForm] ImagemDTO model)
        {
            IList<int> erros = await _produtoService.UploadImagem(model.Id, model.File);
            if (erros.Any())
            {
                return BadRequest(new ErrosDTO { Erros = erros });
            }
            return Ok();
        }



        [AllowAnonymous]
        [HttpGet("todos")]
        public IList<ProdutoDTO> GetProdutos()
        {
            IList<ProdutoDTO> produtos = new List<ProdutoDTO> {
                   new ProdutoDTO{ Nome = "Baguete de Frango", NomeCategoria = "Pão", Preco = 3.25, Ingredientes = new List<string>{ "Pão(Baguete)", "Frango" }, Alergenios = new List<string>{ "Glúten", "Bagas de Goji" }},
                   new ProdutoDTO{ Nome = "Torrada", NomeCategoria = "Pão", Preco = 1, Ingredientes = new List<string>{ "Pão(sementes)","Manteiga" }, Alergenios = new List<string>{ "Sementes de Girassol"}},
                   new ProdutoDTO{ Nome = "Pizza Pepperonni", NomeCategoria = "Pizza", Preco = 6, Ingredientes = new List<string>{ "Pepperoni","Mozzarella" }, Alergenios = new List<string>()},
                   new ProdutoDTO{ Nome = "Arroz de Cabidela", NomeCategoria = "Prato", Preco = 20, Ingredientes = new List<string>{ "Frango","Arroz" }, Alergenios = new List<string>{ "Vinagre" } },
                   new ProdutoDTO{ Nome = "Coca-Cola", NomeCategoria = "Bebida", Preco = 5, Ingredientes = new List<string>(), Alergenios = new List<string>() }
            };
            return produtos;
        }

    }
}