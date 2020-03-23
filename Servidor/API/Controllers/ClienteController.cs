using System;
using API.Business;
using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize]
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
        [HttpPost("autenticacao")]
        public IActionResult Login([FromBody] AutenticacaoModel model)
        {
            var cliente = _clienteService.Login(model.Email, model.Password);

            if (cliente == null)
                return BadRequest(new { message = "Email ou Password incorretos" });

            return Ok(cliente);
        }

        /*
        [HttpPost]
        public async Task<IActionResult> AdicionaCliente(string Nome, string Email, string Password, int NumTelemovel)
        {
             await gestorDadosCliente.CriarConta(Nome, Email, Password, NumTelemovel);
             return Ok("Adicionou");
        }

        */
    }
}
