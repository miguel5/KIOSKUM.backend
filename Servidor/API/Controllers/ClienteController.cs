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
                await _clienteService.CriarConta(model.Nome, model.Email, model.Password, model.NumTelemovel);
                return Ok("Success");
            } catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentOutOfRangeException e)
            {
                return BadRequest(e.Message);
            }
            catch (ArgumentException e)
            {
                return Conflict(e.Message);
            }
        }



        [AllowAnonymous]
        [HttpPost("validar")]
        public IActionResult ValidarConta([FromBody] ValidarClienteDTO model)
        {
            if(model is null)
                return BadRequest(nameof(model));

            try
            {
                bool sucesso = _clienteService.ValidarConta(model.Email, model.Codigo);

                if (sucesso)
                {
                    return Ok("Success");
                }
                return BadRequest("InvalidCode");
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



        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AutenticacaoDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                string token = _clienteService.Login(model.Email, model.Password);

                if (token == null)
                {
                    return Unauthorized(new { message = "LoginFailed" });
                }
                return Ok(new TokenDTO { Token = token});
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
                        return Ok("Success");
                    }
                }
                return Unauthorized("InvalidToken");

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