using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


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
        [HttpPost("authenticate")]
        public IActionResult Login([FromBody] AutenticacaoModel model)
        {
            Console.WriteLine(model.Email + "\n" + model.Password);
            return Ok();
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
