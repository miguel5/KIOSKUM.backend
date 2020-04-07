using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Administrador")]
        [HttpPost("add")]
        public IActionResult AddProduto([FromBody] ProdutoDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult resultado = _produtoService.AddProduto(model);
                if (resultado.Sucesso)
                {
                    return Ok();
                }
                return BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }

        }




        [Authorize(Roles = "Administrador")]
        [HttpPost("upload/imagem")]
        public async Task<IActionResult> UploadImagem([FromForm] ImagemDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            ServiceResult resultado = await _produtoService.UploadImagem(model);
            if (resultado.Sucesso)
            {
                return Ok();
            }
            return BadRequest(resultado.Erros);
        }



        [HttpGet("todos")]
        public IActionResult GetProdutos(string nomeCategoria)
        {
            try
            {
                ServiceResult<IList<ProdutoDTO>> resultado = _produtoService.GetProdutosCategoria(nomeCategoria);
                if (resultado.Sucesso)
                {
                    return Ok(resultado.Resultado);
                }
                return BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpGet]
        public IActionResult GetProduto(int idProduto)
        {
            try
            {
                ServiceResult<ProdutoDTO> resultado = _produtoService.GetProdutoId(idProduto);
                if (resultado.Sucesso)
                {
                    return Ok(resultado.Resultado);
                }
                return BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}