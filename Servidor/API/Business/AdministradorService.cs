using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using API.Data;
using API.Entities;
using API.Helpers;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Business
{
    public interface IAdministradorService
    {

    }

    public class AdministradorService
    {
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IAdministradorDAO _administradorDAO;

        public AdministradorService(IOptions<AppSettings> appSettings, IMapper mapper, IAdministradorDAO administradorDAO)
        {
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _administradorDAO = administradorDAO;
        }


        private string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[0],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }


        private bool ValidaNome(string nome)
        {
            return nome.Length <= 100;
        }

        private bool ValidaEmail(string email)
        {
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            return rx.IsMatch(email) && email.Length <= 100;
        }

        private bool ValidaPassword(string password)
        {
            return password.Length >= 8 && password.Length <= 45;
        }

        private bool ValidaNumFuncionario(int numTelemovel)
        {
            Regex rx = new Regex("^[0-9]{5}$");
            return rx.IsMatch(numTelemovel.ToString());
        }


        public ServiceResult CriarConta(AdministradorDTO model)
        {
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
                erros.Add((int)ErrosEnumeration.EmailJaExiste);

            }
            if (_administradorDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                erros.Add((int)ErrosEnumeration.NumFuncionarioJaExiste);
            }
            if (!ValidaNome(model.Nome))
            {
                erros.Add((int)ErrosEnumeration.NomeInvalido);
            }
            if (!ValidaEmail(model.Email))
            {
                erros.Add((int)ErrosEnumeration.EmailInvalido);
            }
            if (!ValidaPassword(model.Password))
            {
                erros.Add((int)ErrosEnumeration.PasswordInvalida);
            }
            if (!ValidaNumFuncionario(model.NumFuncionario))
            {
                erros.Add((int)ErrosEnumeration.NumFuncionarioInvalido);
            }

            if (!erros.Any())
            {
                Administrador administrador = _mapper.Map<Administrador>(model);
                administrador.Password = HashPassword(model.Password);
                _administradorDAO.InserirAdministrador(administrador);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<TokenDTO> Login(AutenticacaoDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "FieldNull");
            }

            IList<int> erros = new List<int>();
            TokenDTO resultToken = null;
            if (!_administradorDAO.ExisteEmail(model.Email))
            {
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            else
            { 
                if (!erros.Any())
                {
                    Administrador administrador = _administradorDAO.GetAdministradorEmail(model.Email);
                    if (administrador != null && administrador.Password.Equals(HashPassword(model.Password)))
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
                        erros.Add((int)ErrosEnumeration.EmailPasswordIncorreta);
                    }
                }
            }

            return new ServiceResult<TokenDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = resultToken };
        }


    }
}
