using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize(Roles = "Cliente")]
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }


        [AllowAnonymous]
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ClienteDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                Tuple<Email, Email> emails = _clienteService.CriarConta(model.Nome, model.Email, model.Password, model.NumTelemovel);
                EmailSenderService emailSender = new EmailSenderService();
                await emailSender.SendEmail(model.Email, emails.Item1);
                await emailSender.SendEmail(model.Email, emails.Item2);
                return Ok("Sucesso");
            } catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return Conflict(e.Message);
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
                Cliente cliente = _clienteService.Login(model.Email, model.Password);

                if (cliente == null)
                {
                    return Unauthorized(new { message = "Email ou Password incorretos" });
                }
                return Ok(new TokenDTO { Token = cliente.Token });
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("editar")]
        public IActionResult EditarDados([FromBody] ClienteDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (nameIdentifier != null && int.TryParse(nameIdentifier, out int idCliente))
                {
                    bool sucesso = _clienteService.EditarDados(idCliente, model.Nome, model.Email, model.Password, model.NumTelemovel);

                    if (sucesso)
                    {
                        return Ok("Sucesso");
                    }
                }
                return BadRequest("Dados Inseridos inválidos");

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (ArgumentOutOfRangeException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (ArgumentException e)
            {
                return Conflict(new { message = e.Message });
            }
        }


        [HttpGet("get")]
        public IActionResult GetCliente()
        {
            string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (nameIdentifier != null && int.TryParse(nameIdentifier, out int idCliente))
            {
                Cliente cliente = _clienteService.GetCliente(idCliente);
                if (cliente != null)
                {
                    ClienteDTO clienteDTO = new ClienteDTO { Nome = cliente.Nome, Email = cliente.Email, Password = cliente.Password, NumTelemovel = cliente.NumTelemovel };
                    return Ok(clienteDTO);
                }
            }
            return NotFound("ClienteNotFound");

        }
    }
}