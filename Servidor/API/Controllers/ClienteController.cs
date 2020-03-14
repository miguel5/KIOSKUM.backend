using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private List<Cliente> clientes;
        GestorDados gestorDados = new GestorDados();
        private readonly ILogger<ClienteController> logger;

        public ClienteController(ILogger<ClienteController> logger)
        {
            this.logger = logger;
            clientes = new List<Cliente>();
            Cliente c;
            for (int i = 0; i < 5; i++)
            {
                c = new Cliente(i,"Antonio", "tone_biclas@gmail.com", "12345", 924513637);
                clientes.Add(c);
            }
        }


        [HttpGet]
        [Route("Todos")]
        public IList<Cliente> Get()
        {
            return clientes;

        }


        [HttpPost]
        public void AdicionaCliente(string Nome, string Email, string Password, int NumTelemovel)
        {
            if (ValidaNumTelemovel(NumTelemovel))
            {
                Cliente c = new Cliente(0, Nome, Email, Password, NumTelemovel);
                clientes.Add(c);
                gestorDados.CriarConta(Nome, Email, Password, NumTelemovel);
                Console.WriteLine(c.ToString());
            }
        }


        private bool ValidaNumTelemovel(int NumTelemovel)
        {
            Regex rx = new Regex("^9[1236]{1}[0-9]{7}$");
            return rx.IsMatch(NumTelemovel.ToString());

        }
    }
}
