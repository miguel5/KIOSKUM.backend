using System;
using System.Security.Claims;
using API.Business.Interfaces;
using API.Entities;
using API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/administrador")]
    public class AdministradorController : ControllerBase
    {
        private IAdministradorBusiness _administradorBusiness;


        public AdministradorController(IAdministradorBusiness administradorBusiness)
        {
            _administradorBusiness = administradorBusiness;
        }


        [HttpPost("criar")]
        public IActionResult CriarConta([FromBody] AdministradorDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                ServiceResult resultado = _administradorBusiness.CriarConta(model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(e.Message);
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
                ServiceResult<TokenDTO> resultado = _administradorBusiness.Login(model);
                return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpPost("editar")]
        public IActionResult EditarDados([FromBody] EditarAdministradorDTO model)
        {
            if (model is null)
                return BadRequest(nameof(model));

            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idAdministrador = int.Parse(nameIdentifier);
                ServiceResult resultado = _administradorBusiness.EditarDados(idAdministrador, model);
                return resultado.Sucesso ? Ok() : (IActionResult)BadRequest(resultado.Erros);
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
            int idAdministrador = int.Parse(nameIdentifier);
            ServiceResult<AdministradorDTO> resultado = _administradorBusiness.GetAdministrador(idAdministrador);
            return resultado.Sucesso ? Ok(resultado.Resultado) : (IActionResult)BadRequest(resultado.Erros);
        }
    }
}