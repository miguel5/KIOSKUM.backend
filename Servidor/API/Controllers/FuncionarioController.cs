﻿using System;
using API.Business;
using API.Entities;
using API.ViewModels.FuncionarioDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Models
{
    [Authorize(Roles = "Administrador")]
    [ApiController]
    [Route("api/funcionario")]
    public class FuncionarioController : ControllerBase
    {
        private readonly ILogger _logger;
        private IFuncionarioService _funcionarioService;


        public FuncionarioController(ILogger<FuncionarioController> logger, IFuncionarioService funcionarioService)
        {
            _logger = logger;
            _funcionarioService = funcionarioService;
        }


        [HttpPost("criar")]
        public IActionResult CriarConta([FromBody] FuncionarioViewDTO model)
        {
            _logger.LogDebug("A executar api/funcionario/criar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto FuncionarioViewDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _funcionarioService.CriarConta(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"O {model.Nome} e Número de Funcionário {model.NumFuncionario} foi registado com sucesso!");
                    return Ok();
                }
                else
                {
                    _logger.LogDebug("Ocorreu um erro ao criar conta!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }


        [HttpPost("editar")]
        public IActionResult EditarConta([FromBody] FuncionarioViewDTO model)
        {
            _logger.LogDebug("A executar api/funcionario/editar -> Post");
            if (model is null)
            {
                _logger.LogWarning("O objeto FuncionarioViewDTO é null!");
                return BadRequest(nameof(model));
            }

            try
            {
                ServiceResult resultado = _funcionarioService.EditarConta(model);
                if (resultado.Sucesso)
                {
                    _logger.LogInformation($"Foi editado o nome do Funcionário com Número de Funcionário {model.NumFuncionario} para o novo Nome {model.Nome}!");
                    return Ok();
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao editar a conta do Funcionário com Número de Funcionário {model.NumFuncionario}!");
                    return BadRequest(resultado.Erros);
                }
            }
            catch (ArgumentNullException e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500);
            }
        }



        [HttpGet("get")]
        public IActionResult GetFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar api/funcionario/get -> Get");
            try
            {
                ServiceResult<FuncionarioViewDTO> resultado = _funcionarioService.GetFuncionario(numFuncionario);
                if (resultado.Sucesso)
                {
                    _logger.LogDebug($"Get do Funcionário com Número de Funcionário {numFuncionario} efetuado com sucesso!");
                    return Ok(resultado.Resultado);
                }
                else
                {
                    _logger.LogDebug($"Ocorreu um erro ao efetuar a Get do Funcionário com Número de Funcionário {numFuncionario}!");
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