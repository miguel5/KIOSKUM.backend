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
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                ServiceResult resultado = _clienteService.CriarConta(model);
                if (!resultado.Sucesso)
                {
                    return BadRequest(resultado.Erros);
                }
                else
                {
                    ServiceResult<Tuple<Email, Email>> resultadoEmails = _clienteService.GetEmails(model.Email);
                    if (resultadoEmails.Sucesso)
                    {
                        await _emailSenderService.SendEmail(model.Email, resultadoEmails.Resultado.Item1);
                        await _emailSenderService.SendEmail(model.Email, resultadoEmails.Resultado.Item2);
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
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost("confirmar")]
        public IActionResult ConfirmarConta([FromBody] ConfirmarClienteDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                ServiceResult resultado = _clienteService.ConfirmarConta(model);
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