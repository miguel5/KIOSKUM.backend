using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministradorController : ControllerBase
    {
        private List<Administrador> administradores;
        private readonly ILogger<AdministradorController> logger;

        public AdministradorController(ILogger<AdministradorController> logger)
        {
            this.logger = logger;
            administradores = new List<Administrador>();
            Administrador a;
            for (int i = 0; i < 5; i++)
            {
                a = new Administrador(1,"Antonio", "tone_biclas@gmail.com", "12345", 4513637);
                administradores.Add(a);
            }
        }
       


        [HttpGet]
        [Route("Todos")]
        public IList<Administrador> Get()
        {
            return administradores;

        }


        [HttpPost]
        public void AdicionaCliente(string Nome, string Email, string Password, int NumFuncionario)
        {
            Administrador a = new Administrador(1, Nome, Email, Password, NumFuncionario);
            administradores.Add(a);
            Console.WriteLine(a.ToString());
        }
    }
}

