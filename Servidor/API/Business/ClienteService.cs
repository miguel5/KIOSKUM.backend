using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using API.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using API.Helpers;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using API.Data;

namespace API.Business
{
    public interface IClienteService
    {
        Tuple<Email, Email> CriarConta(string nome, string email, string password, int numTelemovel);
        bool ValidaCodigoValidacao(string email, string codigo);
        Cliente Login(string Email, string Password);
        bool EditarDados(int token, string novoNome, string novoEmail, string novaPassword, int numTelemovels);
        Cliente GetCliente(int idCliente);
    }


    public class ClienteService : IClienteService
    {
        private readonly AppSettings _appSettings;
        private readonly ClienteDAO clienteDAO;

        public ClienteService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            clienteDAO = new ClienteDAO();
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
            return nome.Length <= 45;
        }

        private bool ValidaEmail(string email)
        {
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            return rx.IsMatch(email) && email.Length <= 45;
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



        public Tuple<Email,Email> CriarConta(string nome, string email, string password, int numTelemovel)
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

            if (clienteDAO.ExisteEmail(email))
            {
                throw new ArgumentException("EmailRepetido", "Email");
            }

            if (clienteDAO.ExisteNumTelemovel(numTelemovel))
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
            clienteDAO.InserirCliente(cliente, codigoValidacao);

            //string pathEmailBoasVindas = "D:\\home\\site\\wwwroot\\Files\\EmailBoasVindas.json";
            string pathEmailBoasVindas = "/Users/lazaropinheiro/KIOSKUM.backend/Servidor/API/Files/EmailBoasVindas.json";
            StreamReader sr = new StreamReader(pathEmailBoasVindas);
            string json = sr.ReadToEnd();
            Email emailBoasVindas = JsonConvert.DeserializeObject<Email>(json);

            //string pathEmailgerarCodigo = "D:\\home\\site\\wwwroot\\Files\\EmailGerarCodigo.json";
            string pathEmailgerarCodigo = "/Users/lazaropinheiro/KIOSKUM.backend/Servidor/API/Files/EmailGerarCodigo.json";
            sr = new StreamReader(pathEmailgerarCodigo);
            json = sr.ReadToEnd();
            Email emailGerarCodigo = JsonConvert.DeserializeObject<Email>(json);
            emailGerarCodigo.AdcionaCodigo(codigoValidacao);

            Tuple<Email,Email > emails = new Tuple<Email, Email>(emailBoasVindas, emailGerarCodigo);
            return emails;
        }





        public bool ValidaCodigoValidacao(string email, string codigo)
        {
            throw new NotImplementedException();
        }





        public Cliente Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password", "FieldNull");
            }
            if (!clienteDAO.ExisteEmail(email))
            {
                throw new ArgumentException("EmailNotFound", "Email");
            }

            Cliente cliente = clienteDAO.GetClienteEmail(email);
            
             // return null if user not found
            if (cliente == null || cliente.Password.Equals(HashPassword(password)) == false)
            {
                return null;
            }
               

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
            cliente.Token = tokenHandler.WriteToken(token);
            return cliente;
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

            if (clienteDAO.ExisteEmail(novoEmail))
            {
                throw new ArgumentException("EmailRepetido", "Email");
            }

            if (clienteDAO.ExisteNumTelemovel(numTelemovel))
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
            Cliente cliente = clienteDAO.GetClienteId(idCliente);
            if (cliente != null)
            {
                cliente.Nome = novoNome;
                cliente.Email = novoEmail;
                cliente.Password = HashPassword(novaPassword);
                cliente.NumTelemovel = numTelemovel;

                clienteDAO.EditarDados(cliente);
                sucesso = true;
            }
            return sucesso;
        }


        public Cliente GetCliente(int idCliente)
        {
            return clienteDAO.GetClienteId(idCliente);
        }

    }
}