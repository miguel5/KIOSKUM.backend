using System;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.Models;
using API.ModelViews;
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
        [HttpPost("criar-cliente")]
        public async Task<IActionResult> CriarConta([FromBody] ContaClienteModel model)
        {
            bool res = await _clienteService.CriarConta(model.Nome, model.Email, model.Password, model.NumTelemovel);
            if(res == false){
                return BadRequest(new { message = "Dados Inseridos inválidos" });
            }
            return Ok("Adicionou");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AutenticacaoModel model)
        {
            var cliente = _clienteService.Login(model.Email, model.Password);

            if (cliente == null)
                return BadRequest(new { message = "Email ou Password incorretos" });

            ClienteModelView clienteModelView = new ClienteModelView { Nome = cliente.Nome, Email = cliente.Email, NumTelemovel = cliente.NumTelemovel, Token = cliente.Token }; 
            return Ok(clienteModelView);
        }
    }
}
