using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Business;
using API.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private List<Cliente> clientes;
        ClienteService gestorDadosCliente = new ClienteService();
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(ILogger<ClienteController> logger)
        {
            _logger = logger;
            clientes = new List<Cliente>();
            Cliente c;
            for (int i = 0; i < 5; i++)
            {
                c = new Cliente();
                c.IdCliente = i;
                c.Nome = "Antonio";
                c.Email = "tone_biclas@gmail.com";
                c.Password = "12345";
                c.NumTelemovel = 924513637;
                clientes.Add(c);
            }
        }


        [HttpGet]
        [Route("todos")]
        public IList<Cliente> Get()
        {
            return clientes;

        }


        [HttpPost]
        public async Task<IActionResult> AdicionaCliente(string Nome, string Email, string Password, int NumTelemovel)
        {
             await gestorDadosCliente.CriarConta(Nome, Email, Password, NumTelemovel);
             return Ok("Adicionou");
        }

        
    }
}
