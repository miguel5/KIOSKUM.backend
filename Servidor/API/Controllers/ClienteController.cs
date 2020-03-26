using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
                bool res = await _clienteService.CriarConta(model.Nome, model.Email, model.Password, model.NumTelemovel);
                if (res == false)
                {
                    return BadRequest(new { message = "Dados Inseridos inválidos" });
                }
                return Ok("Sucesso");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
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
                return Ok(new TokenDTO { Token =    cliente.Token });
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
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
                if(nameIdentifier != null && int.TryParse(nameIdentifier, out int idCliente))
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
        }
    }
}