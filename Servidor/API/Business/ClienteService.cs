using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace API.Business
{
    public interface IClienteService
    {
        Task CriarConta(string nome, string email, string password, int numTelemovel);
        bool ValidarConta(string email, string codigo);
        string Login(string Email, string Password);
        bool EditarDados(int token, string novoNome, string novoEmail, string novaPassword, int numTelemovels);
        Cliente GetCliente(int idCliente);
    }


    public class ClienteService : IClienteService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppSettings _appSettings;
        private readonly ClienteDAO _clienteDAO;

        public ClienteService(IOptions<AppSettings> appSettings, IWebHostEnvironment webHostEnviroment)
        {
            _webHostEnvironment = webHostEnviroment;
            _appSettings = appSettings.Value;
            _clienteDAO = new ClienteDAO();
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



        public async Task CriarConta(string nome, string email, string password, int numTelemovel)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException("Nome", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password","FieldNull");
            }
            if (_clienteDAO.ExisteEmail(email))
            {
                throw new ArgumentException("EmailRepetido", "Email");
            }

            if (_clienteDAO.ExisteNumTelemovel(numTelemovel))
            {
                throw new ArgumentException("TelemovelRepetido", "NumTelemovel");
            }
            if (!ValidaNome(nome))
            {
                throw new ArgumentOutOfRangeException("Nome", "NomeInvalido");
            }
            if (!ValidaEmail(email))
            {
                throw new ArgumentOutOfRangeException("Email", "EmailInvalido");
            }
            if (!ValidaPassword(password))
            {
                throw new ArgumentOutOfRangeException("Password", "PasswordInvalido");
            }
            if (!ValidaNumTelemovel(numTelemovel))
            {
                throw new ArgumentOutOfRangeException("NumTelemovel", "NumTelemovelInvalido");
            }

            string codigoValidacao = GerarCodigo();
            Cliente cliente = new Cliente { Nome = nome, Email = email, Password = password, NumTelemovel = numTelemovel };
            //clienteDAO.InserirCliente(cliente, codigoValidacao);

            //string pathEmailBoasVindas = "D:\\home\\site\\wwwroot\\Files\\EmailBoasVindas.json";
            string pathEmailBoasVindas = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", "EmailBoasVindas.json");
            StreamReader sr = new StreamReader(pathEmailBoasVindas);
            string json = sr.ReadToEnd();
            Email emailBoasVindas = JsonConvert.DeserializeObject<Email>(json);

            //string pathEmailgerarCodigo = "D:\\home\\site\\wwwroot\\Files\\EmailGerarCodigo.json";
            string pathEmailgerarCodigo = Path.Combine(_webHostEnvironment.ContentRootPath, "Files","EmailGerarCodigo.json");
            sr = new StreamReader(pathEmailgerarCodigo);
            json = sr.ReadToEnd();
            Email emailGerarCodigo = JsonConvert.DeserializeObject<Email>(json);
            emailGerarCodigo.AdcionaCodigo(codigoValidacao);

            EmailSenderService emailSender = new EmailSenderService(_appSettings.EmailSettings);
            await emailSender.SendEmail(email, emailGerarCodigo);
            await emailSender.SendEmail(email, emailBoasVindas);
        }





        public bool ValidarConta(string email, string codigo)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentNullException("Codigo", "FieldNull");
            }
            if (!_clienteDAO.ExisteEmail(email))
            {
                throw new ArgumentException("EmailNotFound", "Email");
            }

            bool sucesso = _clienteDAO.ContaValida(email).Equals(codigo);
            if (sucesso)
            {
                _clienteDAO.ValidarConta(email);
            }
            return sucesso;
        }





        public string Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password", "FieldNull");
            }
            if (!_clienteDAO.ExisteEmail(email))
            {
                throw new ArgumentException("EmailNotFound", "Email");
            }
            if (!_clienteDAO.ContaValida(email))
            {
                throw new ArgumentException("UnverifiedAccount", "Email");
            }

            string resulToken = null;
            Cliente cliente = _clienteDAO.GetClienteEmail(email);
            if (cliente != null && cliente.Password.Equals(HashPassword(password)))
            {
                // authentication successful so generate jwt token
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
                resulToken = tokenHandler.WriteToken(token);
            }
            return resulToken;
        }


        public bool EditarDados(int idCliente, string novoNome, string novoEmail, string novaPassword, int numTelemovel)
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

            if (_clienteDAO.ExisteEmail(novoEmail))
            {
                throw new ArgumentException("EmailRepetido", "Email");
            }

            if (_clienteDAO.ExisteNumTelemovel(numTelemovel))
            {
                throw new ArgumentException("TelemovelRepetido", "NumTelemovel");
            }
            if (!ValidaNome(novoNome))
            {
                throw new ArgumentOutOfRangeException("Nome", "NomeInvalido");
            }
            if (!ValidaEmail(novoEmail))
            {
                throw new ArgumentOutOfRangeException("Email", "EmailInvalido");
            }
            if (!ValidaPassword(novaPassword))
            {
                throw new ArgumentOutOfRangeException("Password", "PasswordInvalido");
            }
            if (!ValidaNumTelemovel(numTelemovel))
            {
                throw new ArgumentOutOfRangeException("NumTelemovel", "NumTelemovelInvalido");
            }

            bool sucesso = false;
            Cliente cliente = _clienteDAO.GetClienteId(idCliente);
            if (cliente != null)
            {
                cliente.Nome = novoNome;
                cliente.Email = novoEmail;
                cliente.Password = HashPassword(novaPassword);
                cliente.NumTelemovel = numTelemovel;

                _clienteDAO.EditarConta(cliente);
                sucesso = true;
            }
            return sucesso;
        }


        public Cliente GetCliente(int idCliente)
        {
            return _clienteDAO.GetClienteId(idCliente);
        }

    }
}