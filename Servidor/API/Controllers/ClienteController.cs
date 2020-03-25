using System;
using System.Linq;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private IClienteService _clienteService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClienteController(IHttpContextAccessor httpContextAccessor,IClienteService clienteService)
        {
            _httpContextAccessor = httpContextAccessor;
            _clienteService = clienteService;
        }

        public bool IsUserLoggedIn()
        {
            var context = _httpContextAccessor.HttpContext;
            return context.User.Identities.Any(x => x.IsAuthenticated);
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
                return Ok("Adicionou");
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
            {
                return BadRequest(nameof(model));
            }

            try
            {
                Cliente cliente = _clienteService.Login(model.Email, model.Password);

                if (cliente == null)
                    return BadRequest(new { message = "Email ou Password incorretos" });

                ClienteDTO clienteDTO = new ClienteDTO { Nome = cliente.Nome, Email = cliente.Email, Password = cliente.Password, NumTelemovel = cliente.NumTelemovel };
                return Ok(clienteDTO);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost("editar/email")]
        public IActionResult EditarDados([FromBody] ClienteDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                string token; 
                Cliente cliente = _clienteService.EditarDados(token, model.Nome, model.Email, model.Password, model.NumTelemovel);

                if (cliente == null)
                    return BadRequest("Dados Inseridos inválidos");

                ClienteDTO clienteModelView = new ClienteDTO { Nome = cliente.Nome, Email = cliente.Email, Password = cliente.Password, NumTelemovel = cliente.NumTelemovel };
                return Ok(clienteModelView);

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}