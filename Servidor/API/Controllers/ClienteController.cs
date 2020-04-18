using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.ViewModels;
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
        private IClienteService _clienteService;
        private IEmailSenderService _emailSenderService;


        public ClienteController(ILogger<ClienteController> logger, IClienteService clienteService, IEmailSenderService emailSenderService)
        {
            _logger = logger;
            _clienteService = clienteService;
            _emailSenderService = emailSenderService;
        }


        [AllowAnonymous]
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ClienteDTO model)
        {
            _logger.LogInformation("A executar api/cliente/criar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto ClienteDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _clienteService.CriarConta(model);
                if (!resultado.Sucesso)
                {
                    _logger.LogWarning("Ocorreu um erro ao criar conta!");
                    return BadRequest(resultado.Erros);
                }
                else
                {
                    _logger.LogInformation($"O {model.Nome}, com {model.Email} e {model.NumTelemovel} registou-se com sucesso!");
                    ServiceResult<Tuple<Email, Email>> resultadoEmails = _clienteService.GetEmails(model.Email);
                    if (resultadoEmails.Sucesso)
                    {
                        await _emailSenderService.SendEmail(model.Email, resultadoEmails.Resultado.Item1);
                        _logger.LogInformation("Email de Boas Vindas enviado com sucesso!");
                        await _emailSenderService.SendEmail(model.Email, resultadoEmails.Resultado.Item2);
                        _logger.LogInformation("Email do Código de Confirmação enviado com sucesso!");
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(resultadoEmails.Erros);
                    }
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
        public IActionResult ConfirmarConta([FromBody] ConfirmarClienteDTO model)
        {
            _logger.LogInformation("A executar api/cliente/confirmar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto ClienteDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _clienteService.ConfirmarConta(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation("Confirmação da conta realizada com sucesso!");
                    return Ok();
                }
                else
                {
                    _logger.LogWarning("Ocorreu um erro ao confirmar conta!");
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
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                ServiceResult<TokenDTO> resultado = _clienteService.Login(model);
                return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        [HttpPost("editar")]
        public IActionResult EditarDados([FromBody] EditarClienteDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idCliente = int.Parse(nameIdentifier);
                ServiceResult resultado = _clienteService.EditarDados(idCliente, model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }


        [HttpGet("get")]
        public IActionResult GetCliente()
        {
            string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int idCliente = int.Parse(nameIdentifier);
            try
            {
                ServiceResult<ClienteDTO> resultado = _clienteService.GetCliente(idCliente);
                return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}