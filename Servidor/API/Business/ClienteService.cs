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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace API.Business
{
    public interface IClienteService
    {
        IList<int> CriarConta(string nome, string email, string password, int numTelemovel);
        Tuple<Email, Email> GetEmails(string email);
        IList<int> ConfirmarConta(string email, string codigo);
        Tuple<IList<int>, TokenDTO> Login(string email, string password);
        IList<int> EditarDados(int idCliente, string novoNome, string novoEmail, string novaPassword, int numTelemovel);
        ClienteDTO GetCliente(int idCliente);
    }


    public class ClienteService : IClienteService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppSettings _appSettings;
        private readonly IClienteDAO _clienteDAO;

        public ClienteService(IOptions<AppSettings> appSettings, IWebHostEnvironment webHostEnviroment, IClienteDAO clienteDAO)
        {
            _webHostEnvironment = webHostEnviroment;
            _appSettings = appSettings.Value;
            _clienteDAO = clienteDAO;
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


        private string GerarCodigo()
        {
            Random random = new Random();
            const string carateres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(carateres, 8).Select(s => s[random.Next(s.Length)]).ToArray());
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

        private bool ValidaNumTelemovel(int numTelemovel)
        {
            Regex rx = new Regex("^9[1236]{1}[0-9]{7}$");
            return rx.IsMatch(numTelemovel.ToString());
        }



        public IList<int> CriarConta(string nome, string email, string password, int numTelemovel)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password","Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();

            if (_clienteDAO.ExisteEmail(email))
            {
               erros.Add(Erros.EmailJaExiste);

            }
            if (_clienteDAO.ExisteNumTelemovel(numTelemovel))
            {
                erros.Add(Erros.NumTelemovelJaExiste);
            }
            if (!ValidaNome(nome))
            {
                erros.Add(Erros.NomeInvalido);
            }
            if (!ValidaEmail(email))
            {
                erros.Add(Erros.EmailInvalido);
            }
            if (!ValidaPassword(password))
            {
                erros.Add(Erros.PasswordInvalida);
            }
            if (!ValidaNumTelemovel(numTelemovel))
            {
                erros.Add(Erros.NumTelemovelInvalido);
            }

            if (!erros.Any()) { 
                string codigoValidacao = GerarCodigo();
                int numMaximoTentativas = _appSettings.NumTentativasCodigoValidacao;
                Cliente cliente = new Cliente { Nome = nome, Email = email, NumTelemovel = numTelemovel };
                cliente.Password = HashPassword(password);
                _clienteDAO.InserirCliente(cliente, codigoValidacao, numMaximoTentativas);
            }
            return erros;
        }


        public Tuple<Email, Email> GetEmails(string email)
        {
            string codigoValidacao = _clienteDAO.GetCodigoValidacao(email);

            string pathEmailBoasVindas = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", "EmailBoasVindas.json");
            StreamReader sr = new StreamReader(pathEmailBoasVindas);
            string json = sr.ReadToEnd();
            Email emailBoasVindas = JsonConvert.DeserializeObject<Email>(json);

            string pathEmailgerarCodigo = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", "EmailGerarCodigo.json");
            sr = new StreamReader(pathEmailgerarCodigo);
            json = sr.ReadToEnd();
            Email emailGerarCodigo = JsonConvert.DeserializeObject<Email>(json);
            emailGerarCodigo.AdcionaCodigo(codigoValidacao);

            return new Tuple<Email, Email>(emailBoasVindas, emailGerarCodigo);
        }

       public IList<int> ConfirmarConta(string email, string codigo)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo!");
            }
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentNullException("Codigo", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();

            if (!_clienteDAO.ExisteEmail(email))
            {
                erros.Add(Erros.EmailNaoExiste);
            }
            if (_clienteDAO.ContaConfirmada(email)) 
            {
                erros.Add(Erros.ContaJaConfirmada);
            }
            if (!_clienteDAO.GetCodigoValidacao(email).Equals(codigo))
            {
                int numTentativas = _clienteDAO.GetNumTentativas(email) + 1;
                if(numTentativas > 0)
                {
                    erros.Add(Erros.CodigoValidacaoErrado);
                }
                else
                {
                    erros.Add(Erros.NumTentativasExcedido);
                }
            }

            if (!erros.Any())
            {
                _clienteDAO.ValidarConta(email);
            }
            return erros;
        }



        public Tuple<IList<int>,TokenDTO> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password", "FieldNull");
            }

            IList<int> erros = new List<int>();
            TokenDTO resultToken = null;

            if (!_clienteDAO.ContaConfirmada(email))
            {
                erros.Add(Erros.ContaNaoConfirmada);
            }
            /*if (!_clienteDAO.ContaAtiva(email))
            {
                erros.Add(Erros.ContaDesativada);
            }*/
            if (!erros.Any())
            {
                Cliente cliente = _clienteDAO.GetClienteEmail(email);
                if (cliente != null && cliente.Password.Equals(HashPassword(password)))
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
                    resultToken = new TokenDTO{Token = tokenHandler.WriteToken(token)};
                }
                else
                {
                     erros.Add(Erros.EmailPasswordIncorreta);
                }
            }
            return new Tuple<IList<int>, TokenDTO>(erros, resultToken);
        }



        public IList<int> EditarDados(int idCliente, string novoNome, string novoEmail, string novaPassword, int numTelemovel)
        {
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentNullException("Nome", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(novoEmail))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(novaPassword))
            {
                throw new ArgumentNullException("Password", "FieldNull");
            }

            IList<int> erros = new List<int>();
            Cliente cliente = _clienteDAO.GetClienteId(idCliente);
            if (cliente == null)
            {
                Console.WriteLine("Erro");
            }
            if (_clienteDAO.ExisteEmail(novoEmail) && !novoEmail.Equals(cliente.Email))
            {
                erros.Add(Erros.EmailJaExiste);
            }

            if (_clienteDAO.ExisteNumTelemovel(numTelemovel) && numTelemovel != cliente.NumTelemovel)
            {
                erros.Add(Erros.NumTelemovelJaExiste);
            }
            if (!ValidaNome(novoNome))
            {
                erros.Add(Erros.NomeInvalido);
            }
            if (!ValidaEmail(novoEmail))
            {
                erros.Add(Erros.EmailInvalido);
            }
            if (!ValidaPassword(novaPassword))
            {
                erros.Add(Erros.PasswordInvalida);
            }
            if (!ValidaNumTelemovel(numTelemovel))
            {
                erros.Add(Erros.NumTelemovelInvalido);
            }


            if (!erros.Any())
            {
                cliente.Nome = novoNome;
                cliente.Email = novoEmail;
                cliente.Password = HashPassword(novaPassword);
                cliente.NumTelemovel = numTelemovel;

                _clienteDAO.EditarConta(cliente);
            }
            return erros;
        }


        public ClienteDTO GetCliente(int idCliente)
        {
            Cliente cliente = _clienteDAO.GetClienteId(idCliente);
            return new ClienteDTO { Nome = cliente.Nome, Email = cliente.Email, Password = cliente.Password, NumTelemovel = cliente.NumTelemovel };
        }


        public void DesativarConta(int idCliente)
        {
            //ClienteDAO.DesativarConta(idCliente);
        }
    }
}