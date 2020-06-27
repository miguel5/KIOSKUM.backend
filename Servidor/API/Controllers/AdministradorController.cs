using System;
using System.Security.Claims;
using Business.Interfaces;
using DTO;
using DTO.TrabalhadorDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace API.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/administrador")]
    public class AdministradorController : ControllerBase
    {
        private readonly ILogger _logger;
        private IAdministradorBusiness _administradorBusiness;


        public AdministradorController(ILogger<AdministradorController> logger, IAdministradorBusiness administradorBusiness)
        {
            _logger = logger;
            _administradorBusiness = administradorBusiness;
        }


        [HttpPost("criar")]
        public IActionResult CriarConta([FromBody] TrabalhadorViewDTO model)
        {
            _logger.LogDebug("A executar api/administrador/criar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto AdministradorViewDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _administradorBusiness.CriarConta(model);
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
            _logger.LogDebug("A executar api/administrador/login -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto AutenticacaoDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult<TokenDTO> resultado = _administradorBusiness.Login(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O Administrador com Número de Funcionário {model.NumFuncionario} efetou login com sucesso.");
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


        [HttpPost("editar")]
        public IActionResult EditarConta([FromBody] EditarTrabalhadorDTO model)
        {
            _logger.LogDebug("A executar api/administrador/editar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto EditarTrabalhadorDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idFuncionario = int.Parse(nameIdentifier);
                ServiceResult resultado = _administradorBusiness.EditarConta(idFuncionario, model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O {model.Nome} e Número de Funcionário {model.NumFuncionario} editou a sua conta com sucesso.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar a edição do Administrador com IdFuncionario {idFuncionario}.");
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
        public IActionResult GetAdministrador()
        {
            _logger.LogDebug("A executar api/administrador/get -> Get");
            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idFuncionario = int.Parse(nameIdentifier);
                ServiceResult<TrabalhadorViewDTO> resultado = _administradorBusiness.GetAdministrador(idFuncionario);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Get do Administrador com IdFuncionario {idFuncionario} efetuado com sucesso.");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao efetuar o Get do Administrador com IdFuncionario {idFuncionario}.");
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