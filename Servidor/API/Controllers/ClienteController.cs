using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Business.Interfaces;
using API.Entities;
using API.Services.EmailSender;
using API.ViewModels;
using API.ViewModels.ClienteDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Authorize(Roles = "Cliente")]
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly ILogger _logger;
        private IClienteBusiness _clienteBusiness;
        private IEmailSenderService _emailSenderService;


        public ClienteController(ILogger<ClienteController> logger, IClienteBusiness clienteBusiness, IEmailSenderService emailSenderService)
        {
            _logger = logger;
            _clienteBusiness = clienteBusiness;
            _emailSenderService = emailSenderService;
        }


        [AllowAnonymous]
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ClienteViewDTO model)
        {
            _logger.LogDebug("A executar api/cliente/criar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto ClienteViewDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _clienteBusiness.CriarConta(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O {model.Nome}, com Email {model.Email} e Número de Telemóvel {model.NumTelemovel} registou-se com sucesso!");
                    ServiceResult<Email> resultadoEmails = _clienteBusiness.GetEmailCodigoValidacao(model.Email);
                    if (resultadoEmails.Sucesso)
                    {
                        await _emailSenderService.SendEmail(model.Email, resultadoEmails.Resultado);
                        _logger.LogDebug("Email do Código de Confirmação enviado com sucesso!");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogDebug("Ocorreu um erro na leitura do Email do Código de Confirmação!");
                        return BadRequest(resultadoEmails.Erros);
                    }
                }
                else
                {
                    _logger.LogDebug("Ocorreu um erro ao criar conta!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e,e.Message);
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                _logger.LogError(e,e.Message);
                return StatusCode(500);
            }
        }


        [AllowAnonymous]
        [HttpPost("confirmar")]
        public async Task<IActionResult> ConfirmarConta([FromBody] ConfirmarClienteDTO model)
        {
            _logger.LogDebug("A executar api/cliente/confirmar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto ConfirmarClienteDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _clienteBusiness.ConfirmarConta(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O Cliente com Email {model.Email} confirmou a sua conta com sucesso!");
                    ServiceResult<Email> resultadoEmails = _clienteBusiness.GetEmailBoasVindas(model.Email);
                    if (resultadoEmails.Sucesso)
                    {
                        await _emailSenderService.SendEmail(model.Email, resultadoEmails.Resultado);
                        _logger.LogDebug("Email de Boas Vindas enviado com sucesso!");
                        return Ok();
                    }
                    else
                    {
                        _logger.LogDebug("Ocorreu um erro na leitura do Email de Boas Vindas!");
                        return BadRequest(resultadoEmails.Erros);
                    }
                }
                else
                {
                    _logger.LogDebug("Ocorreu um erro ao confirmar conta!");
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
        [HttpPost("login")]
        public IActionResult Login([FromBody] AutenticacaoDTO model)
        {
            _logger.LogDebug("A executar api/cliente/login -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto AutenticacaoDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult<TokenDTO> resultado = _clienteBusiness.Login(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O Cliente com Email {model.Email} efetou login com sucesso!");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao efetuar login conta com Email {model.Email}!");
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
        public IActionResult EditarConta([FromBody] EditarClienteDTO model)
        {
            _logger.LogDebug("A executar api/cliente/editar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto EditarClienteDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idCliente = int.Parse(nameIdentifier);
                ServiceResult resultado = _clienteBusiness.EditarConta(idCliente, model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O {model.Nome}, com Email {model.Email} e Número de Telemóvel {model.NumTelemovel} editou a sua conta com sucesso!");
                    return Ok();
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao efetuar a edição do Cliente com IdCliente {idCliente}!");
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
        public IActionResult GetCliente()
        {
            _logger.LogDebug("A executar api/cliente/get -> Get");
            string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int idCliente = int.Parse(nameIdentifier);
            try
            {
                ServiceResult<ClienteViewDTO> resultado = _clienteBusiness.GetCliente(idCliente);
                if (resultado.Sucesso)
                {
                    _logger.LogDebug($"Get do Cliente com IdCliente {idCliente} efetuado com sucesso!");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao efetuar o Get do Cliente com IdCliente {idCliente}!");
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