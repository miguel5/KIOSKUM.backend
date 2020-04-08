﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using API.Data;
using API.Entities;
using API.Helpers;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace API.Business
{
    public interface IFuncionarioService
    {
        ServiceResult CriarConta(FuncionarioDTO model);
    }


    public class FuncionarioService
    {
        private readonly IMapper _mapper;
        private readonly IFuncionarioDAO _funcionarioDAO;

        public FuncionarioService(IMapper mapper, IFuncionarioDAO funcionarioDAO)
        {
            _mapper = mapper;
            _funcionarioDAO = funcionarioDAO;
        }


        private bool ValidaNome(string nome)
        {
            return nome.Length <= 100;
        }


        private bool ValidaNumFuncionario(int numTelemovel)
        {
            Regex rx = new Regex("^[0-9]{5}$");
            return rx.IsMatch(numTelemovel.ToString());
        }

        public ServiceResult CriarConta(FuncionarioDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();

            if (_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);

            }
            if (!ValidaNome(model.Nome))
            {
                erros.Add((int)ErrosEnumeration.NomeInvalido);
            }
            if (!ValidaNumFuncionario(model.NumFuncionario))
            {
                erros.Add((int)ErrosEnumeration.NumFuncionarioInvalido);
            }
           

            if (!erros.Any())
            {
                Funcionario funcionario = _mapper.Map<Funcionario>(model);
                _funcionarioDAO.InserirFuncionario(funcionario);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult EditarConta(FuncionarioDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();
            Funcionario funcionario = _funcionarioDAO.GetFuncionario(model.NumFuncionario);

            if(funcionario == null)
            {
                erros.Add((int)ErrosEnumeration.NumFuncionarioNaoExiste);
            }
            else
            {
                if (_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario) && funcionario.NumFuncionario != model.NumFuncionario)
                {
                    erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);

                }
                if (!ValidaNome(model.Nome))
                {
                    erros.Add((int)ErrosEnumeration.NomeInvalido);
                }

                if (!erros.Any())
                {
                    _funcionarioDAO.EditarNomeFuncionario(model.Nome);
                }
            }
            
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }

    }
}
