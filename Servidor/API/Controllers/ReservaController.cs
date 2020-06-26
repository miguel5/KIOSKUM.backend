using System;
using System.Collections.Generic;
using System.Security.Claims;
using Business.Interfaces;
using Entities;
using DTO.ReservaDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services;

namespace API.Controllers
{

    [ApiController]
    [Route("api/reserva")]
    public class ReservaController : ControllerBase
    {
        private readonly ILogger _logger;
        private IReservaBusiness _reservaBusiness;


        public ReservaController(ILogger<ReservaController> logger, IReservaBusiness reservaBusiness)
        {
            _logger = logger;
            _reservaBusiness = reservaBusiness;
        }


        /*[Authorize(Roles = "Cliente")]
        [HttpPost("registar")]
        public IActionResult RegistarReserva (RegistarReservaDTO model)
        {
            string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int idCliente = int.Parse(nameIdentifier);
            try
            {
                if (model is null)
                {
                    return BadRequest(nameof(model));
                }

                ServiceResult resultado = _reservaBusiness.RegistarReserva(idCliente, model);
                if (resultado.Sucesso)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            /*catch (NumFuncionarioInexistenteException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("aceitar")]
        public IActionResult AceitarReserva(FuncionarioDecideReservaDTO model)
        {
            if(model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _reservaBusiness.FuncionarioDecideReserva(model, true);
                if (resultado.Sucesso)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }

        [HttpPost("rejeitar")]
        public IActionResult RejeitarReserva(FuncionarioDecideReservaDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _reservaBusiness.FuncionarioDecideReserva(model, false);
                if (resultado.Sucesso)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpGet("pendentes")]
        public IActionResult GetReservasPendentes()
        { 
            try
            {
                IList<ReservaViewDTO> reservasPendentes = _reservaBusiness.GetReservasEstado(EstadosReservaEnum.Pendente);
                return Ok(reservasPendentes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpGet("rejeitadas")]
        public IActionResult GetReservasRejeitadas()
        {
            try
            {
                IList<ReservaViewDTO> reservasRejeitadas = _reservaBusiness.GetReservasEstado(EstadosReservaEnum.Rejeitada);
                return Ok(reservasRejeitadas);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpGet("aceites")]
        public IActionResult GetReservasAceites()
        {
            try
            {
                IList<ReservaViewDTO> reservasAceites = _reservaBusiness.GetReservasEstado(EstadosReservaEnum.Aceite);
                return Ok(reservasAceites);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpGet("pagas")]
        public IActionResult GetReservasPagas()
        {
            try
            {
                IList<ReservaViewDTO> reservasPagas = _reservaBusiness.GetReservasEstado(EstadosReservaEnum.Paga);
                return Ok(reservasPagas);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpGet("entregues")]
        public IActionResult GetReservasEntregues()
        {
            try
            {
                IList<ReservaViewDTO> reservasEntregues = _reservaBusiness.GetReservasEstado(EstadosReservaEnum.Entregue);
                return Ok(reservasEntregues);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpGet("canceladas")]
        public IActionResult GetReservasCanceladas()
        {
            try
            {
                IList<ReservaViewDTO> reservasCanceladas = _reservaBusiness.GetReservasEstado(EstadosReservaEnum.Cancelada);
                return Ok(reservasCanceladas);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpPost("entregar")]
        public IActionResult EntregarReserva(EntregarReservaDTO model)
        {
            if (model is null)
            {
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _reservaBusiness.EntregarReserva(model);
                if (resultado.Sucesso)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(resultado.Erros);
                }
            }
            catch (NumFuncionarioInexistenteException)
            {
                return Unauthorized();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }*/
    }
}
