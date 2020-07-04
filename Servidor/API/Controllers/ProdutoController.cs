using System;
using System.Threading.Tasks;
using Business.Interfaces;
using DTO.ProdutoDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;
using Services.Imagem;

namespace API.Controllers
{
    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IProdutoBusiness _produtoBusiness;
        private IImagemService _imagemService;

        public ProdutoController(ILogger<ProdutoController> logger, IWebHostEnvironment webHostEnvironment, IProdutoBusiness produtoBusiness, IImagemService imagemService)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _produtoBusiness = produtoBusiness;
            _imagemService = imagemService;
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("registar")]
        public async Task<IActionResult> RegistarProduto([FromForm] RegistarProdutoDTO model)
        {
            _logger.LogDebug("A executar api/produto/registar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto RegistarProdutoDTO é null!");
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
                    ServiceResult<Tuple<string, string>> resultado = _produtoBusiness.RegistarProduto(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2, _webHostEnvironment.WebRootPath.ToString());
                        _logger.LogInformation($"O Produto com nome {model.Nome}, com o preço {model.Preco} pertencente à categoria com idCategoria {model.IdCategoria} foi registado com sucesso.");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogInformation($"Ocorreu um erro ao registar o Produto com nome {model.Nome}.");
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


        [Authorize(Roles = "Administrador")]
        [HttpPost("editar")]
        public async Task<IActionResult> EditarProduto([FromForm] EditarProdutoDTO model)
        {
            _logger.LogDebug("A executar api/produto/editar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto EditarProdutoDTO é null!");
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
                    ServiceResult<Tuple<string, string>> resultado = _produtoBusiness.EditarProduto(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2, _webHostEnvironment.WebRootPath.ToString());
                        _logger.LogInformation($"O Produto com IdProduto {model.IdProduto} foi editado, com o nome {model.Nome}, com o preço {model.Preco} pertencente à Categoria com IdCategoria {model.IdCategoria}.");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogInformation($"Ocorreu um erro ao editar o Produto com IdProduto {model.IdProduto}.");
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


        [Authorize(Roles = "Administrador")]
        [HttpGet("especifico")]
        public IActionResult GetProduto(int idProduto)
        {
            _logger.LogDebug("A executar api/produto/especifico -> Get");
            try
            {
                ServiceResult<ProdutoViewDTO> resultado = _produtoBusiness.GetProduto(idProduto);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"dFoi efetuado o get do Produto com IdProduto {idProduto}.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o get do Produto com IdProduto {idProduto}.");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("desativar")]
        public IActionResult DesativarProduto(int idProduto)
        {
            _logger.LogDebug("A executar api/produto/desativar -> Post");
            try
            {
                ServiceResult resultado = _produtoBusiness.DesativarProduto(idProduto);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O Produto com IdProduto {idProduto} foi desativado!");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao desativar o Produto com IdProduto {idProduto}.");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("ativar")]
        public IActionResult AtivarProduto(int idProduto)
        {
            _logger.LogDebug("A executar api/produto/ativar -> Post");
            try
            {
                ServiceResult resultado = _produtoBusiness.AtivarProduto(idProduto);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O Produto com IdProduto {idProduto} foi ativado.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao ativar o Produto com IdProduto {idProduto}.");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }
    }
}