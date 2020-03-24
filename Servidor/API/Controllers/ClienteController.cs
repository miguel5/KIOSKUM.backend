using System;
using System.Threading.Tasks;
using API.Business;
using API.Models;
using API.ModelViews;
using API.Entities;
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
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ContaClienteModel model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                bool res = await _clienteService.CriarConta(model.Nome, model.Email, model.Password, model.NumTelemovel);
                if (res == false)
                {
                    return BadRequest(new { message = "Dados Inseridos inválidos" });
                }
                return Ok("Adicionou");
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AutenticacaoModel model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                Cliente cliente = _clienteService.Login(null, model.Password);

                if (cliente == null)
                    return BadRequest(new { message = "Email ou Password incorretos" });

                ClienteModelView clienteModelView = new ClienteModelView { Nome = cliente.Nome, Email = cliente.Email, NumTelemovel = cliente.NumTelemovel, Token = cliente.Token };
                return Ok(clienteModelView);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost("editar/email")]
        public IActionResult EditarEmail([FromBody] EditarEmailModel model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                Cliente cliente = _clienteService.EditarEmail(model.Token, model.Email);

                if (cliente == null)
                    return BadRequest("Dados Inseridos inválidos");

                ClienteModelView clienteModelView = new ClienteModelView { Nome = cliente.Nome, Email = cliente.Email, NumTelemovel = cliente.NumTelemovel, Token = cliente.Token };
                return Ok(clienteModelView);

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }



        [HttpPost("editar/nome")]
        public IActionResult EditarNome([FromBody] EditarNomeModel model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                Cliente cliente = _clienteService.EditarNome(model.Token, model.Nome);

                if (cliente == null)
                    return BadRequest("Dados Inseridos inválidos");

                ClienteModelView clienteModelView = new ClienteModelView { Nome = cliente.Nome, Email = cliente.Email, NumTelemovel = cliente.NumTelemovel, Token = cliente.Token };
                return Ok(clienteModelView);

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpPost("editar/password")]
        public IActionResult EditarPassword([FromBody] EditarPasswordModel model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                Cliente cliente = _clienteService.EditarPassword(model.Token, model.Password);

                if (cliente == null)
                    return BadRequest("Dados Inseridos inválidos");

                ClienteModelView clienteModelView = new ClienteModelView { Nome = cliente.Nome, Email = cliente.Email, NumTelemovel = cliente.NumTelemovel, Token = cliente.Token };
                return Ok(clienteModelView);

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpPost("editar/numTelemovel")]
        public IActionResult EditarNumTelemovel([FromBody] EditarNumTelemovelModel model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                Cliente cliente = _clienteService.EditarNumTelemovel(model.Token, model.NumTelemovel);

                if (cliente == null)
                    return BadRequest("Dados Inseridos inválidos");

                ClienteModelView clienteModelView = new ClienteModelView { Nome = cliente.Nome, Email = cliente.Email, NumTelemovel = cliente.NumTelemovel, Token = cliente.Token };
                return Ok(clienteModelView);

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}