using System;
using Business.Interfaces;
using DTO;
using DTO.TrabalhadorDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace API.Models
{
    [ApiController]
    [Route("api/funcionario")]
    public class FuncionarioController : ControllerBase
    {
        private readonly ILogger _logger;
        private IFuncionarioBusiness _funcionarioBusiness;


        public FuncionarioController(ILogger<FuncionarioController> logger, IFuncionarioBusiness funcionarioBusiness)
        {
            _logger = logger;
            _funcionarioBusiness = funcionarioBusiness;
        }



        [Authorize(Roles = "Administrador")]
        [HttpPost("criar")]
        public IActionResult CriarConta([FromBody] TrabalhadorViewDTO model)
        {
            _logger.LogDebug("A executar api/funcionario/criar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto TrabalhadorViewDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _funcionarioBusiness.CriarConta(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O {model.Nome}, o Número de Funcionário {model.NumFuncionario} registou-se com sucesso.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation("Ocorreu um erro ao criar conta.");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }

        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AutenticacaoTrabalhadorDTO model)
        {
            _logger.LogDebug("A executar api/funcionario/login -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto AutenticacaoTrabalhadorDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult<TokenDTO> resultado = _funcionarioBusiness.Login(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O Funcionário com Número de Funcionário {model.NumFuncionario} efetou login com sucesso.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o login com o Número de Funcionário {model.NumFuncionario}.");
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

        [Authorize(Roles = "Administrador")]
        [HttpPost("editar")]
        public IActionResult EditarConta([FromBody] EditarTrabalhadorDTO model)
        {
            _logger.LogDebug("A executar api/funcionario/editar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto EditarTrabalhadorDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _funcionarioBusiness.EditarConta(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O {model.Nome} e Número de Funcionário {model.NumFuncionario} editou a sua conta com sucesso.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar a edição do Funcionário com Número de Funcionário {model.NumFuncionario}.");
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


        [Authorize(Roles = "Administrador")]
        [HttpGet("get")]
        public IActionResult GetFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar api/funcionario/get -> Get");
            try
            {
                ServiceResult<TrabalhadorViewDTO> resultado = _funcionarioBusiness.GetFuncionario(numFuncionario);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Get do Funcionário com o Número de Funcionário {numFuncionario} efetuado com sucesso.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o Get do Funcionário com Número de Funcionário {numFuncionario}.");
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