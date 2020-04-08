using System;
using API.Business;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Models
{
    [ApiController]
    [Route("api/funcionario")]
    public class FuncionarioController : ControllerBase
    {
        private IFuncionarioService _funcionarioService;

        public FuncionarioController(IFuncionarioService funcionarioService)
        {
            _funcionarioService = funcionarioService;
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("criar")]
        public IActionResult CriarConta([FromBody] FuncionarioDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                ServiceResult resultado = _funcionarioService.CriarConta(model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}