﻿using System;
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
    public class ClienteController : ControllerBase
    {
        private List<Cliente> clientes;
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
            Cliente c = new Cliente(0, Nome, Email, Password, NumTelemovel);
            clientes.Add(c);
            Console.WriteLine(c.ToString());
        }
    }
}