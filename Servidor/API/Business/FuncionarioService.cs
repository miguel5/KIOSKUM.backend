using System;
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

    }


    public class FuncionarioService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IFuncionarioDAO _funcionarioDAO;

        public FuncionarioService(IWebHostEnvironment webHostEnviroment, IMapper mapper, IFuncionarioDAO funcionarioDAO)
        {
            _webHostEnvironment = webHostEnviroment;
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
                throw new ArgumentNullException("Email", "Campo não poder ser nulo.");
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
    }
}
