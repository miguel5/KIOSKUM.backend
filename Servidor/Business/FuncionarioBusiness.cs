using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Business.Interfaces;
using DAO.Interfaces;
using Entities;
using DTO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Services;
using Services.HashPassword;
using Helpers;
using Microsoft.Extensions.Options;
using DTO.TrabalhadorDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Business
{
    public class FuncionarioBusiness : IFuncionarioBusiness
    {
        private readonly ILogger _logger;
        private readonly IHashPasswordService _hashPasswordService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IFuncionarioDAO _funcionarioDAO;


        public FuncionarioBusiness(ILogger<FuncionarioBusiness> logger, IHashPasswordService hashPasswordService, IOptions<AppSettings> appSettings, IMapper mapper, IFuncionarioDAO funcionarioDAO)
        {
            _logger = logger;
            _hashPasswordService = hashPasswordService;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _funcionarioDAO = funcionarioDAO;
        }


        public ServiceResult CriarConta(TrabalhadorViewDTO model)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> CriarConta]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();

            if (!ValidaNome(model.Nome))
            {
                _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                erros.Add((int)ErrosEnumeration.NomeInvalido);
            }
            if (_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} já existe.");
                erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);
            }
            if (!ValidaPassword(model.Password))
            {
                _logger.LogDebug($"A Password {model.Password} é inválida.");
                erros.Add((int)ErrosEnumeration.PasswordInvalida);
            }
            if (!ValidaNumFuncionario(model.NumFuncionario))
            {
                _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} é inválido.");
                erros.Add((int)ErrosEnumeration.NumFuncionarioInvalido);
            }
           
            if (!erros.Any())
            {
                Funcionario funcionario = _mapper.Map<Funcionario>(model);
                funcionario.Password = _hashPasswordService.HashPassword(model.Password);
                _funcionarioDAO.InserirConta(funcionario);
            }

            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<TokenDTO> Login(AutenticacaoTrabalhadorDTO model)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> Login]");
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            TokenDTO resultToken = null;

            if (!_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                _logger.LogWarning($"O Número de Funcionário {model.NumFuncionario} não existe.");
                erros.Add((int)ErrosEnumeration.NumFuncionarioNaoExiste);
            }
            else
            {
                Funcionario funcionario = _funcionarioDAO.GetContaNumFuncionario(model.NumFuncionario);
                if(funcionario == null)
                {
                    _logger.LogWarning($"O Número de Funcionário {model.NumFuncionario} é um Administrador.");
                    erros.Add((int)ErrosEnumeration.NumFuncionarioInvalidoLogin);
                }
                else
                {
                    if (funcionario.Password.Equals(_hashPasswordService.HashPassword(model.Password)))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                            new Claim(ClaimTypes.NameIdentifier, funcionario.IdFuncionario.ToString()),
                            new Claim(ClaimTypes.Role, "Funcionario")
                            }),
                            Expires = DateTime.UtcNow.AddHours(9),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        resultToken = new TokenDTO { Token = tokenHandler.WriteToken(token) };
                    }
                    else
                    {
                        _logger.LogWarning("A Password está incorreta!");
                        erros.Add((int)ErrosEnumeration.NumFuncionarioPasswordIncorreta);
                    }
                }
            }

            return new ServiceResult<TokenDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = resultToken };
        }


        public ServiceResult EditarConta(EditarTrabalhadorDTO model)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> CriarConta]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "Campo não poder ser nulo!");
            }

            if (string.IsNullOrWhiteSpace(model.NovaPassword))
            {
                throw new ArgumentNullException("NovaPassword", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Funcionario funcionario = _funcionarioDAO.GetContaNumFuncionario(model.NumFuncionario);

            if(funcionario == null)
            {
                _logger.LogWarning($"Não existe nenhum Funcionário com Número de Funcionário {model.NumFuncionario}!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                if (!ValidaNome(model.Nome))
                {
                    _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                    erros.Add((int)ErrosEnumeration.NomeInvalido);
                }
                if (!ValidaPassword(model.NovaPassword))
                {
                    _logger.LogDebug("A Password introduzida é inválido.");
                    erros.Add((int)ErrosEnumeration.PasswordInvalida);
                }
                if (!_hashPasswordService.HashPassword(model.Password).Equals(funcionario.Password))
                {
                    _logger.LogDebug("As Passwords não coincidem.");
                    erros.Add((int)ErrosEnumeration.PasswordsNaoCorrespondem);
                }


                if (!erros.Any())
                {
                    Funcionario funcionarioEditado = _mapper.Map<Funcionario>(model);
                    funcionarioEditado.IdFuncionario = funcionario.IdFuncionario;
                    funcionarioEditado.NumFuncionario = funcionario.NumFuncionario;
                    funcionarioEditado.Password = _hashPasswordService.HashPassword(model.NovaPassword);
                    _funcionarioDAO.EditarConta(funcionarioEditado);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }

        public ServiceResult<TrabalhadorViewDTO> GetFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> GetFuncionario]");
            IList<int> erros = new List<int>();
            TrabalhadorViewDTO funcionarioDTO = null;

            Funcionario funcionario = _funcionarioDAO.GetContaNumFuncionario(numFuncionario);
            if (funcionario == null)
            {
                _logger.LogWarning($"Não existe nenhum Funcionário com Número de Funcionário {numFuncionario}!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                funcionarioDTO = _mapper.Map<TrabalhadorViewDTO>(funcionario);
            }

            return new ServiceResult<TrabalhadorViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = funcionarioDTO };
        }



        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> ValidaNome]");
            return nome.Length <= 100;
        }

        private bool ValidaPassword(string password)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> ValidaPassword]");
            return password.Length >= 8 && password.Length <= 45;
        }

        private bool ValidaNumFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar [FuncionarioBusiness -> ValidaNumFuncionario]");
            Regex rx = new Regex("^[0-9]{5}$");
            return rx.IsMatch(numFuncionario.ToString());
        }
    }
}