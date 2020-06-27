using System;
using System.Security.Claims;
using Business.Interfaces;
using DTO;
using DTO.TrabalhadorDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace API.Models
{
    [Authorize(Roles = "Funcionario")]
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
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idFuncionario = int.Parse(nameIdentifier);
                ServiceResult resultado = _funcionarioBusiness.EditarConta(idFuncionario, model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O {model.Nome} e Número de Funcionário {model.NumFuncionario} editou a sua conta com sucesso.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar a edição do Funcionário com IdFuncionario {idFuncionario}.");
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


        [HttpGet("get")]
        public IActionResult GetFuncionario()
        {
            _logger.LogDebug("A executar api/funcionario/get -> Get");
            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idFuncionario = int.Parse(nameIdentifier);
                ServiceResult<TrabalhadorViewDTO> resultado = _funcionarioBusiness.GetFuncionario(idFuncionario);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Get do Funcionário com IdFuncionario {idFuncionario} efetuado com sucesso.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o Get do Funcionário com IdFuncionario {idFuncionario}.");
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