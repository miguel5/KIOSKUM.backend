using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Interfaces;
using Services.Imagem;
using DTO.CategoriaDTOs;
using DTO.ProdutoDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Microsoft.AspNetCore.Hosting;

namespace API.Controllers
{
    [ApiController]
    [Route("api/categoria")]
    public class CategoriaController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ICategoriaBusiness _categoriaBusiness;
        private IImagemService _imagemService;

        public CategoriaController(ILogger<CategoriaController> logger, IWebHostEnvironment webHostEnviroment, ICategoriaBusiness categoriaBusiness, IImagemService imagemService)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnviroment;
            _categoriaBusiness = categoriaBusiness;
            _imagemService = imagemService;
        }



        [AllowAnonymous]
        //[Authorize(Roles = "Administrador")]
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
                    _logger.LogInformation("O ficheiro não é válido para o sistema.");
                    return BadRequest(resultadoValidacaoImagem.Erros);
                }
                else
                {
                    ServiceResult<Tuple<string, string>> resultado = _categoriaBusiness.RegistarCategoria(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2, _webHostEnvironment.WebRootPath);
                        _logger.LogInformation($"A Categoria com nome {model.Nome} foi registado com sucesso.");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogInformation($"Ocorreu um erro ao registar a categoria com nome {model.Nome}.");
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


        [AllowAnonymous]
        //[Authorize(Roles = "Administrador")]
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
                    _logger.LogInformation("O ficheiro não é válido para o sistema.");
                    return BadRequest(resultadoValidacaoImagem.Erros);
                }
                else
                {
                    ServiceResult<Tuple<string, string>> resultado = _categoriaBusiness.EditarCategoria(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2, _webHostEnvironment.WebRootPath);
                        _logger.LogInformation($"A Categoria com  IdCategoria {model.IdCategoria} foi editada, com o nome {model.Nome}.");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogInformation($"Ocorreu um erro ao editar a Categoria com IdCategoria {model.IdCategoria}.");
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
                _logger.LogInformation("Get das Categorias desativadas efetuado com sucesso.");
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



        [AllowAnonymous]
        //[Authorize(Roles = "Administrador,Cliente")]
        [HttpGet("ativadas")]
        public IActionResult GetCategoriasAtivadas()
        {
            _logger.LogDebug("A executar api/categoria/ativadas -> Get");
            try
            {
                IList<CategoriaViewDTO> resultado = _categoriaBusiness.GetCategoriasAtivadas();
                _logger.LogInformation("Get das Categorias ativadas efetuado com sucesso.");
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


        [AllowAnonymous]
        //[Authorize(Roles = "Administrador,Cliente")]
        [HttpGet("produtos/ativados")]
        public IActionResult GetProdutosAtivadosCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar api/categoria/produtos/ativados -> Get");
            try
            {
                ServiceResult<IList<ProdutoViewDTO>> resultado = _categoriaBusiness.GetProdutosAtivadosCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Get dos Ativados Produtos da Categoria com IdCategoria {idCategoria} efetuado com sucesso.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o Get dos Produtos Ativados da Categoria com IdCategoria {idCategoria}.");
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


        [AllowAnonymous]
        //[Authorize(Roles = "Administrador,Cliente")]
        [HttpGet("produtos/desativados")]
        public IActionResult GetProdutosDesativadosCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar api/categoria/produtos/desativados -> Get");
            try
            {
                ServiceResult<IList<ProdutoViewDTO>> resultado = _categoriaBusiness.GetProdutosDesativadosCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Get dos Produtos da Categoria com IdCategoria {idCategoria} efetuado com sucesso.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o Get dos Produtos da Categoria com IdCategoria {idCategoria}.");
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


        [AllowAnonymous]
        //[Authorize(Roles = "Administrador")]
        [HttpGet("especifica")]
        public IActionResult GetCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar api/categoria/especifica -> Get");
            try
            {
                ServiceResult<CategoriaViewDTO> resultado = _categoriaBusiness.GetCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Get do Categoria com IdCategoria {idCategoria} efetuado com sucesso.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o Get da Categoria com IdCategoria {idCategoria}.");
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


        [AllowAnonymous]
        //[Authorize(Roles = "Administrador")]
        [HttpPost("desativar")]
        public IActionResult DesativarCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar api/categoria/desativar -> Post");
            try
            {
                ServiceResult resultado = _categoriaBusiness.DesativarCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"A Categoria com idCategoria {idCategoria} foi desativado!");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao desativar a Categoria com IdCategoria {idCategoria}.");
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


        [AllowAnonymous]
        //[Authorize(Roles = "Administrador")]
        [HttpPost("ativar")]
        public IActionResult AtivarCategoria(int idCategoria)
        {
            _logger.LogDebug("A executar api/categoria/ativar -> Post");
            try
            {
                ServiceResult resultado = _categoriaBusiness.AtivarCategoria(idCategoria);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"A Categoria com IdCategoria {idCategoria} foi ativada.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao ativar a Categoria com IdCategoria {idCategoria}.");
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