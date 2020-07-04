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


        [Authorize(Roles = "Cliente")]
        [HttpPost("registar")]
        public IActionResult RegistarReserva ([FromBody] RegistarReservaDTO model)
        {
            _logger.LogDebug("A executar api/reserva/registar -> Post");

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
                    _logger.LogInformation($"O cliente com IdCliente {idCliente} realizou um pedido de Reserva.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation("Ocorreu um erro no pedido de Reserva.");
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

        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpPost("aceitar")]
        public IActionResult AceitarReserva(int idReserva)
        {
            _logger.LogDebug("A executar api/reserva/aceitar -> Post");

            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idFuncionario = int.Parse(nameIdentifier);

                ServiceResult resultado = _reservaBusiness.FuncionarioDecideReserva(idFuncionario, idReserva, true);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"A Reserva com IdReserva {idReserva} foi aceitada.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro ao aceitar a Reserva com IdReserva {idReserva}.");
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

        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpPost("rejeitar")]
        public IActionResult RejeitarReserva(int idReserva)
        {
            _logger.LogDebug("A executar api/reserva/rejeitar -> Post");
            try
            {
                string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                int idFuncionario = int.Parse(nameIdentifier);

                ServiceResult resultado = _reservaBusiness.FuncionarioDecideReserva(idFuncionario, idReserva, false);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"A Reserva com IdReserva {idReserva} foi rejeitada.");
                    return Ok();
                }
                else
                { 
                    _logger.LogInformation($"Ocorreu um erro ao rejeitar a Reserva com IdReserva {idReserva}.");
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


        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpGet("pendentes")]
        public IActionResult GetReservasPendentes()
        {
            _logger.LogDebug("A executar api/reserva/pendentes -> Get");

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


        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpGet("rejeitadas")]
        public IActionResult GetReservasRejeitadas()
        {
            _logger.LogDebug("A executar api/reserva/rejeitadas -> Get");

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


        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpGet("aceitadas")]
        public IActionResult GetReservasAceites()
        {
            _logger.LogDebug("A executar api/reserva/aceitadas -> Get");

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


        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpGet("pagas")]
        public IActionResult GetReservasPagas()
        {
            _logger.LogDebug("A executar api/reserva/pagas -> Get");

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


        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpGet("entregues")]
        public IActionResult GetReservasEntregues()
        {
            _logger.LogDebug("A executar api/reserva/entregues -> Get");

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


        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpGet("canceladas")]
        public IActionResult GetReservasCanceladas()
        {
            _logger.LogDebug("A executar api/reserva/canceladas -> Get");

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


        [Authorize(Roles = "Funcionario,Administrador")]
        [HttpPost("entregar")]
        public IActionResult EntregarReserva(int idReserva)
        {
            _logger.LogDebug("A executar api/reserva/entregar -> Post");

            string nameIdentifier = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int idFuncionario = int.Parse(nameIdentifier);

            try
            {
                ServiceResult resultado = _reservaBusiness.EntregarReserva(idFuncionario, idReserva);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"A Reserva com IdReserva {idReserva} foi entregue.");
                    return Ok();
                }
                else
                {
                    _logger.LogInformation($"Ocorreu um erro na entrega da Reserva com IdReserva {idReserva}.");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }
    }
}