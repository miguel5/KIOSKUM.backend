using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Business;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize(Roles = "Cliente")]
    [ApiController]
    [Route("api/cliente")]
    public class ClienteController : ControllerBase
    {
        private IClienteService _clienteService;
        private IEmailSenderService _emailSenderService;

        public ClienteController(IClienteService clienteService, IEmailSenderService emailSenderService)
        {
            _clienteService = clienteService;
            _emailSenderService = emailSenderService;
        }


        [AllowAnonymous]
        [HttpPost("criar")]
        public async Task<IActionResult> CriarConta([FromBody] ClienteDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                IList<int> erros = _clienteService.CriarConta(model.Nome, model.Email, model.Password, model.NumTelemovel);
                if (erros.Any())
                {
                    return BadRequest(new ErrosDTO { ListaErros = erros });
                }
                Tuple<Email, Email> emails = _clienteService.GetEmails(model.Email);
                await _emailSenderService.SendEmail(model.Email, emails.Item1);
                await _emailSenderService.SendEmail(model.Email, emails.Item2);
                return Ok();

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
        }



        [AllowAnonymous]
        [HttpPost("confirmar")]
        public IActionResult ConfirmarConta([FromBody] ValidarClienteDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                IList<int> erros = _clienteService.ConfirmarConta(model.Email, model.Codigo);
                return erros.Any() ? BadRequest(erros) : (IActionResult)Ok();
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }



        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AutenticacaoDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                Tuple<IList<int>, TokenDTO> resultado = _clienteService.Login(model.Email, model.Password);
                return resultado.Item1.Any() ? BadRequest(new ErrosDTO { ListaErros = resultado.Item1 }) : (IActionResult)Ok(resultado.Item2);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("editar")]
        public IActionResult EditarDados([FromBody] ClienteDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idCliente = int.Parse(nameIdentifier);
                IList<int> erros = _clienteService.EditarDados(idCliente, model.Nome, model.Email, model.Password, model.NumTelemovel);
                return erros.Any() ? BadRequest(erros) : (IActionResult)Ok();

            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpGet("get")]
        public IActionResult GetCliente()
        {
            string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int idCliente = int.Parse(nameIdentifier);
            ClienteDTO clienteDTO = _clienteService.GetCliente(idCliente);
            return Ok(clienteDTO);
        }

    }
}