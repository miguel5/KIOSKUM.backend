using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.ViewModels;
using API.ViewModels.ProdutoDTOs;
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

        [Authorize(Roles = "Administrador")]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduto([FromBody] RegistarProdutoDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult<int> resultado = _produtoService.RegistarProduto(model);
                if (resultado.Sucesso)
                {
                    ServiceResult resultadoUpload = await _produtoService.UploadImagem(resultado.Resultado,model.File);
                    if (resultado.Sucesso)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(resultadoUpload.Erros);
                    }
                }
                else
                {
                    return BadRequest(resultado.Erros);
                } 
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("editar")]
        public async Task<IActionResult> EditarProduto([FromBody] EditarProdutoDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult resultado = _produtoService.EditarProduto(model);
                if (resultado.Sucesso)
                {
                    ServiceResult resultadoUpload = await _produtoService.UploadImagem(model.IdProduto, model.File);
                    if (resultado.Sucesso)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(resultadoUpload.Erros);
                    }
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }

        }


        [Authorize(Roles = "Administrador,Cliente")]
        [HttpGet("todos")]
        public IActionResult GetProdutosCategoria(int idCategoria)
        {
            try
            {
                ServiceResult<IList<ProdutoViewDTO>> resultado = _produtoService.GetProdutosCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    return Ok(resultado.Resultado);
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }




        [Authorize(Roles = "Administrador")]
        [HttpGet("desativados")]
        public IActionResult GetProdutosDesativados()
        {
            try
            {
                ServiceResult<IList<ProdutoViewDTO>> resultado = _produtoService.GetProdutosDesativados();
                if (resultado.Sucesso)
                {
                    return Ok(resultado.Resultado);
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }



        [Authorize(Roles = "Administrador")]
        [HttpGet("especifico")]
        public IActionResult GetProduto(int idProduto)
        {
            try
            {
                ServiceResult<ProdutoViewDTO> resultado = _produtoService.GetProduto(idProduto);
                if (resultado.Sucesso)
                {
                    return Ok(resultado.Resultado);
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }



        [Authorize(Roles = "Administrador")]
        [HttpPost("desativar")]
        public IActionResult DesativarProduto(int idProduto)
        {
            try
            {
                ServiceResult resultado = _produtoService.DesativarProduto(idProduto);
                if (resultado.Sucesso)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("ativar")]
        public IActionResult AtivarProduto(int idProduto)
        {
            try
            {
                ServiceResult resultado = _produtoService.AtivarProduto(idProduto);
                if (resultado.Sucesso)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}