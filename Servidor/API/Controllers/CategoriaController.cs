using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Business.Interfaces;
using API.Entities;
using API.Services.Imagem;
using API.ViewModels.CategoriaDTOs;
using API.ViewModels.ProdutoDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/categoria")]
    public class CategoriaController : ControllerBase
    {
        private readonly ILogger _logger;
        private ICategoriaBusiness _categoriaBusiness;
        private IImagemService _imagemService;


        public CategoriaController(ILogger<CategoriaController> logger, ICategoriaBusiness categoriaBusiness, IImagemService imagemService)
        {
            _logger = logger;
            _categoriaBusiness = categoriaBusiness;
            _imagemService = imagemService;
        }



        //[Authorize(Roles = "Administrador")]
        [AllowAnonymous]
        [HttpPost("registar")]
        public async Task<IActionResult> RegistarCategoria([FromForm] RegistarCategoriaDTO model)
        {
            _logger.LogDebug("A executar api/categoria/registar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto RegistarCategoriaDTO é null!");
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult<string> resultadoValidacaoImagem = _imagemService.ValidaImagem(model.File);
                if (!resultadoValidacaoImagem.Sucesso)
                {
                    _logger.LogDebug("O ficheiro não é válido para o sistema!");
                    return BadRequest(resultadoValidacaoImagem.Erros);
                }
                else
                {
                    ServiceResult<Tuple<string, string>> resultado = _categoriaBusiness.RegistarCategoria(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2);
                        _logger.LogInformation($"A Categoria com nome {model.Nome} foi registado com sucesso!");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogDebug($"Ocorreu um erro ao registar a categoria com nome {model.Nome}!");
                        return BadRequest(resultado.Erros);
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        //[Authorize(Roles = "Administrador")]
        [AllowAnonymous]
        [HttpPost("editar")]
        public async Task<IActionResult> EditarCategoria([FromForm] EditarCategoriaDTO model)
        {
            _logger.LogDebug("A executar api/categoria/editar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto EditarCategoriaDTO é null!");
                return BadRequest(nameof(model));
            }
            try
            {
                ServiceResult<string> resultadoValidacaoImagem = _imagemService.ValidaImagem(model.File);
                if (!resultadoValidacaoImagem.Sucesso)
                {
                    _logger.LogDebug("O ficheiro não é válido para o sistema!");
                    return BadRequest(resultadoValidacaoImagem.Erros);
                }
                else
                {
                    ServiceResult<Tuple<string, string>> resultado = _categoriaBusiness.EditarCategoria(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2);
                        _logger.LogInformation($"A Categoria com  IdCategoria {model.IdCategoria} foi editada, com o nome {model.Nome}!");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogDebug($"Ocorreu um erro ao editar a Categoria com IdCategoria {model.IdCategoria}!");
                        return BadRequest(resultado.Erros);
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        //[Authorize(Roles = "Administrador")]
        [AllowAnonymous]
        [HttpGet("desativadas")]
        public IActionResult GetCategoriasDesativadas()
        {
            _logger.LogDebug("A executar api/categoria/desativadas -> Get");
            try
            {
                IList<CategoriaViewDTO> resultado = _categoriaBusiness.GetCategoriasDesativadas();
                return Ok(resultado);

            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }



        //[Authorize(Roles = "Administrador,Cliente")]
        [AllowAnonymous]
        [HttpGet("todas")]
        public IActionResult GetCategorias()
        {
            _logger.LogDebug("A executar api/categoria/todas -> Get");
            try
            {
                IList<CategoriaViewDTO> resultado = _categoriaBusiness.GetCategorias();
                return Ok(resultado);

            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        //[Authorize(Roles = "Administrador,Cliente")]
        [AllowAnonymous]
        [HttpGet("produtos")]
        public IActionResult GetProdutosCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar api/categoria/produtos -> Get");
            try
            {
                ServiceResult<IList<ProdutoViewDTO>> resultado = _categoriaBusiness.GetProdutosCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    _logger.LogDebug($"Foi efetuado o get dos Produtos da Categoria com IdCategoria {idCategoria}!");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao efetuar o get dos Produtos da Categoria com IdCategoria {idCategoria}!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }



        //[Authorize(Roles = "Administrador")]
        [AllowAnonymous]
        [HttpGet("especifica")]
        public IActionResult GetCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar api/categoria/especifica -> Get");
            try
            {
                ServiceResult<CategoriaViewDTO> resultado = _categoriaBusiness.GetCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    _logger.LogDebug($"Foi efetuado o get do Categoria com IdCategoria {idCategoria}!");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao efetuar o get da Categoria com IdCategoria {idCategoria}!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }
    }
}