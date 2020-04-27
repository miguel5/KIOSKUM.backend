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
    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        //[Authorize(Roles = "Administrador")]
        [AllowAnonymous]
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
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        //[Authorize(Roles = "Administrador")]
        [AllowAnonymous]
        [HttpPost("upload/imagem")]
        public async Task<IActionResult> UploadImagem([FromForm] ImagemDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult resultado = await _produtoService.UploadImagem(model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("editar")]
        public IActionResult EditarProduto([FromBody] ProdutoDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult resultado = _produtoService.EditarProduto(model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }

        }


        [Authorize(Roles = "Administrador,Cliente")]
        [HttpGet("todos")]
        public IActionResult GetProdutosCategoria(string nomeCategoria)
        {
            try
            {
                ServiceResult<IList<ProdutoDTO>> resultado = _produtoService.GetProdutosCategoria(nomeCategoria);
                return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [AllowAnonymous]
        //[Authorize(Roles = "Administrador")]
        [HttpGet("especifico")]
        public IActionResult GetProduto(string nome)
        {
            try
            {
                ServiceResult<ProdutoDTO> resultado = _produtoService.GetProdutoNome(nome);
                return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}