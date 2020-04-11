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
    [Route("api/categoria")]
    public class CategoriaController : ControllerBase
    {
        private ICategoriaService _categoriaService;


        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("add")]
        public IActionResult AddCategoria([FromBody] CategoriaDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult resultado = _categoriaService.AddCategoria(model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
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
            try
            {
                ServiceResult resultado = await _categoriaService.UploadImagem(model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);

            }catch(ArgumentException e)
            {
                return BadRequest(new { message = e.Message });
            }

        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("editar")]
        public IActionResult EditarCategoria([FromBody] CategoriaDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult resultado = _categoriaService.EditarCategoria(model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }

        }


        [Authorize(Roles = "Administrador,Cliente")]
        [HttpGet("todas")]
        public IActionResult GetCategorias()
        {
            ServiceResult<IList<CategoriaDTO>> resultado = _categoriaService.GetTodasCategorias();
            return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
        }


        [Authorize(Roles = "Administrador")]
        [HttpGet("especifica")]
        public IActionResult GetProduto(string nome)
        {
            try
            {
                ServiceResult<CategoriaDTO> resultado = _categoriaService.GetCategoriaNome(nome);
                return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [AllowAnonymous]
        //[Authorize(Roles = "Administrador")]
        [HttpPost("remover")]
        public async Task<IActionResult> RemoverCategoria(string nome)
        { 
            try
            {
                ServiceResult resultado = await _categoriaService.RemoverCategoria(nome);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);

            }
            catch (ArgumentException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
