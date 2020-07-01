using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using Business.Interfaces;
using DAO.Interfaces;
using DTO;
using DTO.ClienteDTOs;
using Entities;
using Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services;
using Services.HashPassword;

namespace Business
{
    public class ClienteBusiness : IClienteBusiness
    {
        private readonly ILogger _logger;
        private readonly IHashPasswordService _hashPasswordService;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IClienteDAO _clienteDAO;

        public ClienteBusiness(ILogger<ClienteBusiness> logger, IHashPasswordService hashPasswordService, IOptions<AppSettings> appSettings, IMapper mapper, IClienteDAO clienteDAO)
        {
            _logger = logger;
            _hashPasswordService = hashPasswordService;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _clienteDAO = clienteDAO;
        }

        public ServiceResult CriarConta(ClienteViewDTO model)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> CriarConta]");
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

            IList<int> erros = new List<int>();

            if (_clienteDAO.ExisteEmail(model.Email))
            {
                _logger.LogDebug($"O Email {model.Email} já existe.");
                erros.Add((int)ErrosEnumeration.EmailJaExiste);
            }
            if (_clienteDAO.ExisteNumTelemovel(model.NumTelemovel))
            {
                _logger.LogDebug($"O Número de Telemóvel {model.NumTelemovel} já existe.");
                erros.Add((int)ErrosEnumeration.NumTelemovelJaExiste);
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
                _logger.LogDebug("A Password introduzida é inválida.");
                erros.Add((int)ErrosEnumeration.PasswordInvalida);
            }
            if (!ValidaNumTelemovel(model.NumTelemovel))
            {
                _logger.LogDebug($"O Número de Telemóvel {model.NumTelemovel} é inválida.");
                erros.Add((int)ErrosEnumeration.NumTelemovelInvalido);
            }

            if (!erros.Any()) { 
                string codigoValidacao = GerarCodigo();
                int numMaximoTentativas = _appSettings.NumTentativasCodigoValidacao;
                Cliente cliente = _mapper.Map<Cliente>(model);
                cliente.Password = _hashPasswordService.HashPassword(model.Password);
                _clienteDAO.InserirConta(cliente, codigoValidacao, numMaximoTentativas);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<Email> GetEmailCodigoValidacao(string email, string contentRootPath)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> GetEmailCodigoValidacao]");
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Email emailCodigoValidacao = null;
            string codigoValidacao = _clienteDAO.GetCodigoValidacao(email);

            if(codigoValidacao != null)
            {
                _logger.LogDebug("Início da leitura do ficheiro de EmailGerarCodigo.json.");
                string pathEmailgerarCodigo = Path.Combine(contentRootPath, "Files", "EmailGerarCodigo.json");
                StreamReader sr = new StreamReader(pathEmailgerarCodigo);
                string json = sr.ReadToEnd();
                _logger.LogDebug("Fim da leitura do ficheiro de EmailGerarCodigo.json.");
                emailCodigoValidacao = JsonConvert.DeserializeObject<Email>(json);
                emailCodigoValidacao.AdicionaCodigo(codigoValidacao);
            }
            else
            {
                _logger.LogWarning($"O Email {email} não existe!");
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            return new ServiceResult<Email> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = emailCodigoValidacao };
        }

       public ServiceResult ConfirmarConta(ConfirmarClienteDTO model)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> ConfirmarConta]");
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo!");
            }
            if (string.IsNullOrWhiteSpace(model.Codigo))
            {
                throw new ArgumentNullException("Codigo", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();

            if (!_clienteDAO.ExisteEmail(model.Email))
            {
                _logger.LogWarning($"O Email {model.Email} não existe!");
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            else
            {
                if (_clienteDAO.ContaConfirmada(model.Email))
                {
                    _logger.LogDebug($"O Email {model.Email} já se encontra confirmado.");
                    erros.Add((int)ErrosEnumeration.ContaJaConfirmada);
                }
                if (!_clienteDAO.GetCodigoValidacao(model.Email).Equals(model.Codigo))
                {
                    _logger.LogDebug($"Código de Validação errado para o Email {model.Email}.");
                    int numTentativas = _clienteDAO.GetNumTentativas(model.Email);
                    if (numTentativas > 0)
                    {
                        _logger.LogDebug($"O Código de Validação está errado. restam-lhe {numTentativas-1} tentativas.");
                        erros.Add((int)ErrosEnumeration.CodigoValidacaoErrado);
                        _clienteDAO.DecrementaTentativas(model.Email);
                    }
                    else
                    {
                        _logger.LogDebug("O Código de Validação está errado. Cliente eliminado.");
                        erros.Add((int)ErrosEnumeration.NumTentativasExcedido);
                    }
                }
                if (!erros.Any())
                {
                    _clienteDAO.ValidarConta(model.Email);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }

        public ServiceResult<Email> GetEmailBoasVindas(string email, string contentRootPath)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> GetEmailBoasVindas]");
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();
            Email emailBoasVindas = null;

            if (_clienteDAO.ExisteEmail(email))
            {
                _logger.LogDebug("Início da leitura do ficheiro de EmailBoasVindas.json.");
                string pathEmailBoasVindas = Path.Combine(contentRootPath, "Files", "EmailBoasVindas.json");
                StreamReader sr = new StreamReader(pathEmailBoasVindas);
                string json = sr.ReadToEnd();
                _logger.LogDebug("Fim da leitura do ficheiro de EmailBoasVindas.json.");
                emailBoasVindas = JsonConvert.DeserializeObject<Email>(json);
            }
            else
            {
                _logger.LogWarning($"O Email {email} não existe!");
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            return new ServiceResult<Email> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = emailBoasVindas };
        }

        public ServiceResult<TokenDTO> Login(AutenticacaoClienteDTO model)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> Login]");
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
            if (!_clienteDAO.ExisteEmail(model.Email))
            {
                _logger.LogWarning($"O Email {model.Email} não existe.");
                erros.Add((int)ErrosEnumeration.EmailPasswordIncorreta);
            }
            else
            {
                if (!_clienteDAO.ContaConfirmada(model.Email))
                {
                    _logger.LogDebug($"O Email {model.Email} não se encontra confirmado.");
                    erros.Add((int)ErrosEnumeration.ContaNaoConfirmada);
                }


                Cliente cliente = _clienteDAO.GetContaEmail(model.Email);
                if (!erros.Any())
                {
                    if (cliente.Password.Equals(_hashPasswordService.HashPassword(model.Password)))
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                    new Claim(ClaimTypes.NameIdentifier, cliente.IdCliente.ToString()),
                    new Claim(ClaimTypes.Role, "Cliente")
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
            }

            return new ServiceResult<TokenDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = resultToken };
        }



        public ServiceResult EditarConta(int idCliente, EditarClienteDTO model)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> EditarDados]");
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
            Cliente cliente = _clienteDAO.GetContaId(idCliente);
            if (cliente == null)
            {
                _logger.LogWarning($"Não existe nenhum Cliente com o IdCliente {idCliente}!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                if (_clienteDAO.ExisteEmail(model.Email) && !model.Email.Equals(cliente.Email))
                {
                    _logger.LogDebug($"O Email {model.Email} já existe.");
                    erros.Add((int)ErrosEnumeration.EmailJaExiste);
                }

                if (_clienteDAO.ExisteNumTelemovel(model.NumTelemovel) && model.NumTelemovel != cliente.NumTelemovel)
                {
                    _logger.LogDebug($"O Número de Telemóvel {model.NumTelemovel} já existe.");
                    erros.Add((int)ErrosEnumeration.NumTelemovelJaExiste);
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
                    _logger.LogDebug("A Password introduzida é inválida.");
                    erros.Add((int)ErrosEnumeration.PasswordInvalida);
                }
                if (!ValidaNumTelemovel(model.NumTelemovel))
                {
                    _logger.LogDebug($"O Número de Telemóvel {model.NumTelemovel} é inválido.");
                    erros.Add((int)ErrosEnumeration.NumTelemovelInvalido);
                }
                if (!_hashPasswordService.HashPassword(model.Password).Equals(cliente.Password))
                {
                    _logger.LogDebug("As Passwords não coincidem.");
                    erros.Add((int)ErrosEnumeration.PasswordsNaoCorrespondem);
                }

                if (!erros.Any())
                {
                    Cliente c = _mapper.Map<Cliente>(model);
                    c.Password = _hashPasswordService.HashPassword(model.NovaPassword);
                    c.IdCliente = idCliente;
                    _clienteDAO.EditarConta(c);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<ClienteViewDTO> GetCliente(int idCliente)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> GetCliente]");
            IList<int> erros = new List<int>();
            ClienteViewDTO clienteDTO = null;

            Cliente cliente = _clienteDAO.GetContaId(idCliente);
            if(cliente == null)
            {
                _logger.LogWarning($"O IdCliente {idCliente} não existe!");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                clienteDTO = _mapper.Map<ClienteViewDTO>(cliente);
            }

            return new ServiceResult<ClienteViewDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = clienteDTO };
        }




        private string GerarCodigo()
        {
            _logger.LogDebug("A executar [ClienteBusiness -> GerarCodigo]");
            Random random = new Random();
            const string carateres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(carateres, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> ValidaNome]");
            return nome.Length <= 100;
        }

        private bool ValidaEmail(string email)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> ValidaEmail]");
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            return rx.IsMatch(email) && email.Length <= 100;
        }

        private bool ValidaPassword(string password)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> ValidaPassword]");
            return password.Length >= 8 && password.Length <= 45;
        }

        private bool ValidaNumTelemovel(int numTelemovel)
        {
            _logger.LogDebug("A executar [ClienteBusiness -> ValidaNumTelemovel]");
            Regex rx = new Regex("^9[1236]{1}[0-9]{7}$");
            return rx.IsMatch(numTelemovel.ToString());
        }



    }
}