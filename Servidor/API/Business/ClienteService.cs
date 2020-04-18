using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace API.Business
{
    public interface IClienteService
    {
        ServiceResult CriarConta(ClienteDTO model);
        ServiceResult<Email> GetEmailCodigoValidacao(string email);
        ServiceResult ConfirmarConta(ConfirmarClienteDTO model);
        ServiceResult<Email> GetEmailBoasVindas(string email);
        ServiceResult<TokenDTO> Login(AutenticacaoDTO model);
        ServiceResult EditarDados(int idCliente, EditarClienteDTO model);
        ServiceResult<ClienteDTO> GetCliente(int idCliente);
    }


    public class ClienteService : IClienteService
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IClienteDAO _clienteDAO;

        public ClienteService(ILogger<ClienteService> logger, IOptions<AppSettings> appSettings, IWebHostEnvironment webHostEnviroment, IMapper mapper, IClienteDAO clienteDAO)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _webHostEnvironment = webHostEnviroment;
            _mapper = mapper;
            _clienteDAO = clienteDAO;
        }


        private string HashPassword(string password)
        {
            _logger.LogDebug("A executar [ClienteService -> HashPassword]");
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[0],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }


        private string GerarCodigo()
        {
            _logger.LogDebug("A executar [ClienteService -> GerarCodigo]");
            Random random = new Random();
            const string carateres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(carateres, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private bool ValidaNome(string nome)
        {
            _logger.LogDebug("A executar [ClienteService -> ValidaNome]");
            return nome.Length <= 100;
        }

        private bool ValidaEmail(string email)
        {
            _logger.LogDebug("A executar [ClienteService -> ValidaEmail]");
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            return rx.IsMatch(email) && email.Length <= 100;
        }

        private bool ValidaPassword(string password)
        {
            _logger.LogDebug("A executar [ClienteService -> ValidaPassword]");
            return password.Length >= 8 && password.Length <= 45;
        }

        private bool ValidaNumTelemovel(int numTelemovel)
        {
            _logger.LogDebug("A executar [ClienteService -> ValidaNumTelemovel]");
            Regex rx = new Regex("^9[1236]{1}[0-9]{7}$");
            return rx.IsMatch(numTelemovel.ToString());
        }



        public ServiceResult CriarConta(ClienteDTO model)
        {
            _logger.LogDebug("A executar [ClienteService -> CriarConta]");
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
                throw new ArgumentNullException("Password","Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();

            if (_clienteDAO.ExisteEmail(model.Email))
            {
                _logger.LogDebug("O email introduzido já existe!");
                erros.Add((int)ErrosEnumeration.EmailJaExiste);
            }
            if (_clienteDAO.ExisteNumTelemovel(model.NumTelemovel))
            {
                _logger.LogDebug("O número de telemóvel introduzido ja existe!");
                erros.Add((int)ErrosEnumeration.NumTelemovelJaExiste);
            }
            if (!ValidaNome(model.Nome))
            {
                _logger.LogDebug("O nome introduzido é inválida!");
                erros.Add((int)ErrosEnumeration.NomeInvalido);
            }
            if (!ValidaEmail(model.Email))
            {
                _logger.LogDebug("O email introduzido é inválida!");
                erros.Add((int)ErrosEnumeration.EmailInvalido);
            }
            if (!ValidaPassword(model.Password))
            {
                _logger.LogDebug("A password introduzida é inválida!");
                erros.Add((int)ErrosEnumeration.PasswordInvalida);
            }
            if (!ValidaNumTelemovel(model.NumTelemovel))
            {
                _logger.LogDebug("O número de telemóvel introduzida é inválida!");
                erros.Add((int)ErrosEnumeration.NumTelemovelInvalido);
            }

            if (!erros.Any()) { 
                string codigoValidacao = GerarCodigo();
                int numMaximoTentativas = _appSettings.NumTentativasCodigoValidacao;
                Cliente cliente = _mapper.Map<Cliente>(model);
                cliente.Password = HashPassword(model.Password);
                _clienteDAO.InserirConta(cliente, codigoValidacao, numMaximoTentativas);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<Email> GetEmailCodigoValidacao(string email)
        {
            _logger.LogDebug("A executar [ClienteService -> GetEmailCodigoValidacao]");
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();
            Email emailCodigoValidacao = null;
            string codigoValidacao = _clienteDAO.GetCodigoValidacao(email);

            if(codigoValidacao != null)
            {
                _logger.LogDebug("Início da leitura do ficheiro de EmailGerarCodigo.json");
                string pathEmailgerarCodigo = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", "EmailGerarCodigo.json");
                StreamReader sr = new StreamReader(pathEmailgerarCodigo);
                string json = sr.ReadToEnd();
                _logger.LogDebug("Fim da leitura do ficheiro de EmailGerarCodigo.json");
                emailCodigoValidacao = JsonConvert.DeserializeObject<Email>(json);
                emailCodigoValidacao.AdicionaCodigo(codigoValidacao);
            }
            else
            {
                _logger.LogWarning($"Não existe código de validação associado ao email {email}!");
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            return new ServiceResult<Email> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = emailCodigoValidacao };
        }

       public ServiceResult ConfirmarConta(ConfirmarClienteDTO model)
        {
            _logger.LogDebug("A executar [ClienteService -> ConfirmarConta]");
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
                _logger.LogDebug($"O email {model.Email} não existe!");
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            else
            {
                if (_clienteDAO.ContaConfirmada(model.Email))
                {
                    _logger.LogDebug("A conta já se encontra confirmada");
                    erros.Add((int)ErrosEnumeration.ContaJaConfirmada);
                }
                if (!_clienteDAO.GetCodigoValidacao(model.Email).Equals(model.Codigo))
                {
                    int numTentativas = _clienteDAO.GetNumTentativas(model.Email);
                    if (numTentativas > 0)
                    {
                        _logger.LogDebug($"O código de validação está errado, tem mais {numTentativas-1} tentativas!");
                        erros.Add((int)ErrosEnumeration.CodigoValidacaoErrado);
                        _clienteDAO.DecrementaTentativas(model.Email);
                    }
                    else
                    {
                        _logger.LogDebug("O código de validação está errado, a conta foi eliminada!");
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

        public ServiceResult<Email> GetEmailBoasVindas(string email)
        {
            _logger.LogDebug("A executar [ClienteService -> GetEmailBoasVindas]");
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();
            Email emailBoasVindas = null;

            if (_clienteDAO.ExisteEmail(email))
            {
                _logger.LogDebug("Início da leitura do ficheiro de EmailBoasVindas.json");
                string pathEmailBoasVindas = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", "EmailBoasVindas.json");
                StreamReader sr = new StreamReader(pathEmailBoasVindas);
                string json = sr.ReadToEnd();
                _logger.LogDebug("Fim da leitura do ficheiro de EmailBoasVindas.json");
                emailBoasVindas = JsonConvert.DeserializeObject<Email>(json);
            }
            else
            {
                _logger.LogWarning($"Não existe código de validação associado ao email {email}!");
                erros.Add((int)ErrosEnumeration.EmailNaoExiste);
            }
            return new ServiceResult<Email> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = emailBoasVindas };
        }

        public ServiceResult<TokenDTO> Login(AutenticacaoDTO model)
        {
            _logger.LogDebug("A executar [ClienteService -> Login]");
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
            if (!_clienteDAO.ExisteEmail(model.Email))
            {
                _logger.LogDebug($"O email {model.Email} não existe!");
                erros.Add((int)ErrosEnumeration.EmailPasswordIncorreta);
            }
            else
            {
                if (!_clienteDAO.ContaConfirmada(model.Email))
                {
                    _logger.LogDebug($"A conta com o email {model.Email} ainda não foi confirmada");
                    erros.Add((int)ErrosEnumeration.ContaNaoConfirmada);
                }


                Cliente cliente = _clienteDAO.GetContaEmail(model.Email);
                if(cliente == null)
                {
                    _logger.LogWarning($"Não existe nenhum utilizador com o email {model.Email}");
                }

                if (!erros.Any())
                {
                    if (cliente.Password.Equals(HashPassword(model.Password)))
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
                        _logger.LogWarning("A password está incorreta");
                        erros.Add((int)ErrosEnumeration.EmailPasswordIncorreta);
                    }
                }
            }

            return new ServiceResult<TokenDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = resultToken };
        }



        public ServiceResult EditarDados(int idCliente, EditarClienteDTO model)
        {
            _logger.LogDebug("A executar [ClienteService -> EditarDados]");
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                throw new ArgumentNullException("Password", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(model.NovaPassword))
            {
                throw new ArgumentNullException("Nova Password", "FieldNull");
            }

            IList<int> erros = new List<int>();
            Cliente cliente = _clienteDAO.GetContaId(idCliente);
            if (cliente == null)
            {
                _logger.LogWarning($"Não existe nenhum utilizador com o id {idCliente}");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                if (_clienteDAO.ExisteEmail(model.Email) && !model.Email.Equals(cliente.Email))
                {
                    _logger.LogDebug($"O email {model.Email} já existe");
                    erros.Add((int)ErrosEnumeration.EmailJaExiste);
                }

                if (_clienteDAO.ExisteNumTelemovel(model.NumTelemovel) && model.NumTelemovel != cliente.NumTelemovel)
                {
                    _logger.LogDebug($"O Número de Telemóvel {model.NumTelemovel} já existe!");
                    erros.Add((int)ErrosEnumeration.NumTelemovelJaExiste);
                }
                if (!ValidaNome(model.Nome))
                {
                    _logger.LogDebug("O nome introduzido é inválida!");
                    erros.Add((int)ErrosEnumeration.NomeInvalido);
                }
                if (!ValidaEmail(model.Email))
                {
                    _logger.LogDebug("O email introduzido é inválida!");
                    erros.Add((int)ErrosEnumeration.EmailInvalido);
                }
                if (!ValidaPassword(model.NovaPassword))
                {
                    _logger.LogDebug("A password introduzido é inválida!");
                    erros.Add((int)ErrosEnumeration.PasswordInvalida);
                }
                if (!ValidaNumTelemovel(model.NumTelemovel))
                {
                    _logger.LogDebug("O número de telemóvel introduzido é inválido!");
                    erros.Add((int)ErrosEnumeration.NumTelemovelInvalido);
                }
                if (!HashPassword(model.Password).Equals(cliente.Password))
                {
                    _logger.LogDebug("As passwords não coincidem!");
                    erros.Add((int)ErrosEnumeration.PasswordsNaoCorrespondem);
                }

                if (!erros.Any())
                {
                    Cliente c = _mapper.Map<Cliente>(model);
                    c.Password = HashPassword(model.NovaPassword);
                    c.IdCliente = idCliente;
                    _clienteDAO.EditarConta(c);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<ClienteDTO> GetCliente(int idCliente)
        {

            IList<int> erros = new List<int>();
            ClienteDTO clienteDTO = null;

            Cliente cliente = _clienteDAO.GetContaId(idCliente);
            if(cliente == null)
            {
                _logger.LogWarning($"Não existe nenhum utilizador com o id {idCliente}");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            else
            {
                clienteDTO = _mapper.Map<ClienteDTO>(cliente);
            }

            return new ServiceResult<ClienteDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = clienteDTO };
        }
    }
}