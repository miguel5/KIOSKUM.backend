﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Models
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private List<Funcionario> funcionarios;
        private readonly ILogger<FuncionarioController> logger;

        public FuncionarioController(ILogger<FuncionarioController> logger)
        {
            this.logger = logger;
            funcionarios = new List<Funcionario>();
            Funcionario f;
            for (int i = 0; i < 5; i++)
            {
                f = new Funcionario("Antonio", "/azure/Massa_com_Atum.jpeg", 4513637);
                funcionarios.Add(f);
            }
        }



        [HttpGet]
        [Route("Todos")]
        public IList <Funcionario> Get()
        {
            return funcionarios;

        }
    }
}