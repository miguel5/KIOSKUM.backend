using System;
using Business.Interfaces;
using DAO.Interfaces;
using Helpers;
using DTO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;
using Services.HashPassword;
using DTO.TrabalhadorDTOs;
using System.Collections.Generic;
using Entities;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Business
{
    public class AdministradorBusiness : IAdministradorBusiness
    {
        private readonly ILogger _logger;
        private readonly IHashPasswordService _hashPasswordService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IAdministradorDAO _administradorDAO;


        public AdministradorBusiness(ILogger<AdministradorBusiness> logger, IHashPasswordService hashPasswordService, IOptions<AppSettings> appSettings, IMapper mapper, IAdministradorDAO administradorDAO)
        {
            _logger = logger;
            _hashPasswordService = hashPasswordService;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _administradorDAO = administradorDAO;
        }


        public ServiceResult CriarConta(TrabalhadorViewDTO model)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> CriarConta]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo.");
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
            if (_administradorDAO.ExisteNumFuncionario(model.NumFuncionario))
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
                Administrador administrador = _mapper.Map<Administrador>(model);
                administrador.Password = _hashPasswordService.HashPassword(model.Password);
                _administradorDAO.InserirConta(administrador);
            }

            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<TokenDTO> Login(AutenticacaoTrabalhadorDTO model)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> Login]");
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            TokenDTO resultToken = null;

            if (!_administradorDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                _logger.LogWarning($"O Número de Funcionário {model.NumFuncionario} não existe.");
                erros.Add((int)ErrosEnumeration.NumFuncionarioNaoExiste);
            }
            else
            { 
                Administrador administrador = _administradorDAO.GetContaNumFuncionario(model.NumFuncionario);
                if(administrador == null)
                {
                    _logger.LogWarning($"O Número de Funcionário {model.NumFuncionario} é um Funcionário.");
                    erros.Add((int)ErrosEnumeration.NumFuncionarioInvalidoLogin);
                }
                else
                {
                    if (administrador.Password.Equals(_hashPasswordService.HashPassword(model.Password)))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                            new Claim(ClaimTypes.NameIdentifier, administrador.IdFuncionario.ToString()),
                            new Claim(ClaimTypes.Role, "Administrador")
                            }),
                            Expires = DateTime.UtcNow.AddHours(12),
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


        public ServiceResult EditarConta(int idFuncionario, EditarTrabalhadorDTO model)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> EditarConta]");
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
            Administrador administrador = _administradorDAO.GetContaIdFuncionario(idFuncionario);
            if (administrador == null)
            {
                _logger.LogWarning($"Não existe nenhum Administrador com o IdFuncionario {idFuncionario}!");
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
                if (!_hashPasswordService.HashPassword(model.Password).Equals(administrador.Password))
                {
                    _logger.LogDebug("As Passwords não coincidem.");
                    erros.Add((int)ErrosEnumeration.PasswordsNaoCorrespondem);
                }


                if (!erros.Any())
                {
                    Administrador administradorEditado = _mapper.Map<Administrador>(model);
                    administradorEditado.IdFuncionario = administrador.IdFuncionario;
                    administradorEditado.NumFuncionario = administrador.NumFuncionario;
                    administradorEditado.Password = _hashPasswordService.HashPassword(model.NovaPassword);
                    _administradorDAO.EditarConta(administradorEditado);
                }
            }
            
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }

        public ServiceResult<TrabalhadorViewDTO> GetAdministrador(int idFuncionario)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> GetAdministrador]");
            IList<int> erros = new List<int>();
            TrabalhadorViewDTO administradorDTO = null;

            Administrador administrador = _administradorDAO.GetContaIdFuncionario(idFuncionario);
            if (administrador == null)
            {
                _logger.LogWarning($"Não existe nenhum Funcionário com Número de Funcionário {idFuncionario}!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                administradorDTO = _mapper.Map<TrabalhadorViewDTO>(administrador);
            }

            return new ServiceResult<TrabalhadorViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = administradorDTO };
        }



        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> ValidaNome]");
            return nome.Length <= 100;
        }

        private bool ValidaPassword(string password)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> ValidaPassword]");
            return password.Length >= 8 && password.Length <= 45;
        }

        private bool ValidaNumFuncionario(int numFuncionario)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> ValidaNumFuncionario]");
            Regex rx = new Regex("^[0-9]{5}$");
            return rx.IsMatch(numFuncionario.ToString());
        }
    }
}