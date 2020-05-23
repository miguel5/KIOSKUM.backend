using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Business.Interfaces;
using API.Entities;
using API.Services.Imagem;
using API.ViewModels.ProdutoDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly ILogger _logger;
        private IProdutoBusiness _produtoBusiness;
        private IImagemService _imagemService;

        public ProdutoController(ILogger<ProdutoController> logger, IProdutoBusiness produtoBusiness, IImagemService imagemService)
        {
            _logger = logger;
            _produtoBusiness = produtoBusiness;
            _imagemService = imagemService;
        }

        //Authorize(Roles = "Administrador")]
        [AllowAnonymous]
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
                    _logger.LogDebug("O ficheiro não é válido para o sistema!");
                    return BadRequest(resultadoValidacaoImagem.Erros);
                }
                else
                {
                    ServiceResult<Tuple<string, string>> resultado = _produtoBusiness.RegistarProduto(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2);
                        _logger.LogInformation($"O Produto com nome {model.Nome}, com o preço {model.Preco} pertencente à categoria com idCategoria {model.IdCategoria} foi registado com sucesso!");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogDebug($"Ocorreu um erro ao registar o Produto com nome {model.Nome}!");
                        return BadRequest(resultado.Erros);
                    }
                }
            }
            catch (ArgumentNullException e)
            {
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
                    _logger.LogDebug("O ficheiro não é válido para o sistema!");
                    return BadRequest(resultadoValidacaoImagem.Erros);
                }
                else
                {
                    ServiceResult<Tuple<string, string>> resultado = _produtoBusiness.EditarProduto(model, resultadoValidacaoImagem.Resultado);
                    if (resultado.Sucesso)
                    {
                        await _imagemService.GuardarImagem(model.File, resultado.Resultado.Item1, resultado.Resultado.Item2);
                        _logger.LogInformation($"O Produto com IdProduto {model.IdProduto} foi editado, com o nome {model.Nome}, com o preço {model.Preco} pertencente à Categoria com IdCategoria {model.IdCategoria}!");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogDebug($"Ocorreu um erro ao editar o Produto com IdProduto {model.IdProduto}!");
                        return BadRequest(resultado.Erros);
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [Authorize(Roles = "Administrador")]
        [HttpGet("desativados")]
        public IActionResult GetProdutosDesativados()
        {
            _logger.LogDebug("A executar api/produto/desativados -> Get");
            try
            {
                IList<ProdutoViewDTO> resultado = _produtoBusiness.GetProdutosDesativados();
                return Ok(resultado);

            }
            catch (ArgumentNullException e)
            {
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
                    _logger.LogDebug($"Foi efetuado o get do Produto com IdProduto {idProduto}!");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao efetuar o get do Produto com IdProduto {idProduto}!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
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
                    _logger.LogInformation($"O Produto com idProduto {idProduto} foi desativado!");
                    return Ok();
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao desativar o Produto com idProduto {idProduto}!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
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
                    _logger.LogInformation($"O Produto com idProduto {idProduto} foi ativado!");
                    return Ok();
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao ativar o Produto com idProduto {idProduto}!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
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