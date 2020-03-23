using System;
using System.Collections.Generic;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{
    [ApiController]
    [Route("api/administrador")]
    public class AdministradorController : ControllerBase
    {
        private List<Administrador> administradores;
        private readonly ILogger<AdministradorController> _logger;

        public AdministradorController(ILogger<AdministradorController> logger)
        {
            _logger = logger;
            administradores = new List<Administrador>();
            Administrador a;
            for (int i = 0; i < 5; i++)
            {
                a = new Administrador();
                a.IdFuncionario = 1;
                a.Nome = "Antonio";
                a.NumFuncionario = 123;
                a.Email = "tone_biclas@gmail.com";
                a.Password = "12345";
                administradores.Add(a);
            }
        }



        [HttpGet]
        [Route("todos")]
        public IList<Administrador> Get()
        {
            return administradores;

        }


        [HttpPost]
        public void AdicionaCliente(string Nome, string Email, string Password, int NumFuncionario)
        {
            Administrador a = new Administrador();
            a.IdFuncionario = 1;
            a.Nome = Nome;
            a.NumFuncionario = NumFuncionario;
            a.Email = Email;
            a.Password = Password;
            administradores.Add(a);
            Console.WriteLine(a.ToString());
        }
    }
}