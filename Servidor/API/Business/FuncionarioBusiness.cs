using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using API.Business.Interfaces;
using API.Data.Interfaces;
using API.Entities;
using API.ViewModels;
using API.ViewModels.FuncionarioDTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace API.Business
{
    public class FuncionarioBusiness : IFuncionarioBusiness
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IFuncionarioDAO _funcionarioDAO;


        public FuncionarioBusiness(ILogger<FuncionarioBusiness> logger, IMapper mapper, IFuncionarioDAO funcionarioDAO)
        {
            _logger = logger;
            _mapper = mapper;
            _funcionarioDAO = funcionarioDAO;
        }


        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> ValidaNome]");
            return nome.Length <= 100;
        }


        private bool ValidaNumFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> ValidaNumFuncionario]");
            Regex rx = new Regex("^[0-9]{5}$");
            return rx.IsMatch(numFuncionario.ToString());
        }


        public ServiceResult CriarConta(FuncionarioViewDTO model)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> CriarConta]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();

            if (_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} já existe.");
                erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);

            }
            if (!ValidaNome(model.Nome))
            {
                _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                erros.Add((int)ErrosEnumeration.NomeInvalido);
            }
            if (!ValidaNumFuncionario(model.NumFuncionario))
            {
                _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} é inválido.");
                erros.Add((int)ErrosEnumeration.NumFuncionarioInvalido);
            }
           
            if (!erros.Any())
            {
                Funcionario funcionario = _mapper.Map<Funcionario>(model);
                _funcionarioDAO.InserirConta(funcionario);
            }

            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult EditarConta(FuncionarioViewDTO model)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> CriarConta]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Funcionario funcionario = _funcionarioDAO.GetContaNumFuncionario(model.NumFuncionario);

            if(funcionario == null)
            {
                _logger.LogWarning($"Não existe nenhum Funcionário com Número de Funcionário {model.NumFuncionario}!");
                erros.Add((int)ErrosEnumeration.NumFuncionarioNaoExiste);
            }
            else
            {
                if (_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario) && funcionario.NumFuncionario != model.NumFuncionario)
                {
                    _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} já existe.");
                    erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);

                }
                if (!ValidaNome(model.Nome))
                {
                    _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                    erros.Add((int)ErrosEnumeration.NomeInvalido);
                }

                if (!erros.Any())
                {
                    _funcionarioDAO.EditarConta(funcionario);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }

        public ServiceResult<FuncionarioViewDTO> GetFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> GetFuncionario]");
            IList<int> erros = new List<int>();
            FuncionarioViewDTO funcionarioDTO = null;

            Funcionario funcionario = _funcionarioDAO.GetContaNumFuncionario(numFuncionario);
            if (funcionario == null)
            {
                _logger.LogWarning($"Não existe nenhum Funcionário com Número de Funcionário {numFuncionario}!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                funcionarioDTO = _mapper.Map<FuncionarioViewDTO>(funcionario);
            }

            return new ServiceResult<FuncionarioViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = funcionarioDTO };
        }
    }
}