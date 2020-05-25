using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using API.Business.Interfaces;
using API.Data.Interfaces;
using API.Entities;
using API.Helpers;
using API.ViewModels;
using API.ViewModels.AdministradorDTOs;
using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Business
{
    public class AdministradorBusiness : IAdministradorBusiness
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IAdministradorDAO _administradorDAO;

        public AdministradorBusiness(ILogger<AdministradorBusiness> logger, IOptions<AppSettings> appSettings, IMapper mapper, IAdministradorDAO administradorDAO)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _administradorDAO = administradorDAO;
        }


        private string HashPassword(string password)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> HashPassword]");
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[0],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }


        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> ValidaNome]");
            return nome.Length <= 100;
        }

        private bool ValidaEmail(string email)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> ValidaEmail]");
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            return rx.IsMatch(email) && email.Length <= 100;
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


        public ServiceResult CriarConta(AdministradorViewDTO model)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> CriarConta]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo.");
            }
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo.");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();

            if (_administradorDAO.ExisteEmail(model.Email))
            {
                _logger.LogDebug($"O Email {model.Email} já existe.");
                erros.Add((int)ErrosEnumeration.EmailJaExiste);
            }
            if (_administradorDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} já existe.");
                erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);
            }
            if (!ValidaNome(model.Nome))
            {
                _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                erros.Add((int)ErrosEnumeration.NomeInvalido);
            }
            if (!ValidaEmail(model.Email))
            {
                _logger.LogDebug($"O Email {model.Email} é inválido.");
                erros.Add((int)ErrosEnumeration.EmailInvalido);
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
                administrador.Password = HashPassword(model.Password);
                _administradorDAO.InserirConta(administrador);
            }

            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<TokenDTO> Login(AutenticacaoDTO model)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> Login]");
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo!");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            TokenDTO resultToken = null;
            if (!_administradorDAO.ExisteEmail(model.Email))
            {
                _logger.LogWarning($"O Email {model.Email} não existe.");
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            else
            { 
                Administrador administrador = _administradorDAO.GetContaEmail(model.Email);
                if (administrador.Password.Equals(HashPassword(model.Password)))
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
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    resultToken = new TokenDTO { Token = tokenHandler.WriteToken(token) };
                }
                else
                {
                    _logger.LogWarning("A Password está incorreta!");
                    erros.Add((int)ErrosEnumeration.EmailPasswordIncorreta);
                }
            }

            return new ServiceResult<TokenDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = resultToken };
        }



        public ServiceResult EditarConta(int idFuncionario, EditarAdministradorDTO model)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> EditarConta]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Campo não poder ser nulo!");
            }
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo!");
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
            Administrador administrador = _administradorDAO.GetContaId(idFuncionario);
            if (administrador == null)
            {
                _logger.LogWarning($"Não existe nenhum Administrador com o IdFuncionario {idFuncionario}!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                if (_administradorDAO.ExisteEmail(model.Email) && !model.Email.Equals(administrador.Email))
                {
                    _logger.LogDebug($"O Email {model.Email} já existe.");
                    erros.Add((int)ErrosEnumeration.EmailJaExiste);
                }

                if (_administradorDAO.ExisteNumFuncionario(model.NumFuncionario) && model.NumFuncionario != administrador.NumFuncionario)
                {
                    _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} já existe.");
                    erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);
                }
                if (!ValidaNome(model.Nome))
                {
                    _logger.LogDebug($"O Nome {model.Nome} é inválido.");
                    erros.Add((int)ErrosEnumeration.NomeInvalido);
                }
                if (!ValidaEmail(model.Email))
                {
                    _logger.LogDebug($"O Email {model.Email} é inválido.");
                    erros.Add((int)ErrosEnumeration.EmailInvalido);
                }
                if (!ValidaPassword(model.NovaPassword))
                {
                    _logger.LogDebug("A Password introduzida é inválido.");
                    erros.Add((int)ErrosEnumeration.PasswordInvalida);
                }
                if (!ValidaNumFuncionario(model.NumFuncionario))
                {
                    _logger.LogDebug($"O Número de Funcionário {model.NumFuncionario} é inválido.");
                    erros.Add((int)ErrosEnumeration.NumFuncionarioInvalido);
                }
                if (!HashPassword(model.Password).Equals(administrador.Password))
                {
                    _logger.LogDebug("As Passwords não coincidem.");
                    erros.Add((int)ErrosEnumeration.PasswordsNaoCorrespondem);
                }


                if (!erros.Any())
                {
                    Administrador a = _mapper.Map<Administrador>(model);
                    a.Password = HashPassword(model.NovaPassword);
                    _administradorDAO.EditarConta(a);
                }
            }
            
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<AdministradorViewDTO> GetAdministrador(int idFuncionario)
        {
            _logger.LogDebug("A executar [AdministradorBusiness -> GetAdministrador]");
            IList<int> erros = new List<int>();
            AdministradorViewDTO administradorDTO = null;

            Administrador administrador = _administradorDAO.GetContaId(idFuncionario);
            if (administrador == null)
            {
                _logger.LogWarning($"O IdFuncionario {idFuncionario} não existe!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                administradorDTO = _mapper.Map<AdministradorViewDTO>(administrador);
            }

            return new ServiceResult<AdministradorViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = administradorDTO };
        }
    }
}
