using System.Collections.Generic;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Models
{
    [ApiController]
    [Route("api/funcionario")]
    public class FuncionarioController : ControllerBase
    {
        private List<Funcionario> funcionarios;
        private readonly ILogger<FuncionarioController> _logger;

        public FuncionarioController(ILogger<FuncionarioController> logger)
        {
            _logger = logger;
            funcionarios = new List<Funcionario>();
            Funcionario f;
            for (int i = 0; i < 5; i++)
            {
                f = new Funcionario();
                f.IdFuncionario = i;
                f.Nome = "Antonio";
                f.NumFuncionario = 4513637;
                funcionarios.Add(f);
            }
        }



        [HttpGet]
        [Route("todos")]
        public IList <Funcionario> Get()
        {
            return funcionarios;

        }
    }
}