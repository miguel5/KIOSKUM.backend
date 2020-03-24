using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using API.Data;
using System.Text.RegularExpressions;
using API.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using API.Helpers;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API.Business
{
    public interface IClienteService
    {
        Task<bool> CriarConta(string nome, string email, string password, int numTelemovel);
        bool ValidaCodigoValidacao(string email, string codigo);
        Cliente Login(string Email, string Password);
        Cliente EditarEmail(string token, string novoEmail);
        Cliente EditarNome(string token, string novoNome);
        Cliente EditarPassword(string token, string novaPassword);
        Cliente EditarNumTelemovel(string token, int numTelemovel);
    }


    public class ClienteService : IClienteService
    {
        private readonly AppSettings _appSettings;
        private ClienteDAO clienteDAO;

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


        private bool ValidaEmail(string Email)
        {
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            return rx.IsMatch(Email);
        }

        private bool ValidaPassword(string Password)
        {
            return Password.Length >= 8;
        }

        private bool ValidaNumTelemovel(int NumTelemovel)
        {
            Regex rx = new Regex("^9[1236]{1}[0-9]{7}$");
            return rx.IsMatch(NumTelemovel.ToString());
        }


        public async Task<bool> CriarConta(string Nome, string Email, string Password, int NumTelemovel)
        {
            bool sucesso = false;
            if (string.IsNullOrWhiteSpace(Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentNullException("Email", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentNullException("Password", "Parametro não pode ser nulo");
            }

            if (ValidaEmail(Email) && ValidaPassword(Password) && ValidaNumTelemovel(NumTelemovel)) //&&
                //!clienteDAO.ExisteEmail(Email) && !clienteDAO.ExisteNumTelemovel(NumTelemovel))
            {
                sucesso = true;
                string codigoValidacao = GerarCodigo();

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

                EmailSenderService emailSender = new EmailSenderService();
                await emailSender.SendEmail(Email, emailGerarCodigo);
                await emailSender.SendEmail(Email, emailBoasVindas);
            }
            return sucesso;
        }


        public bool ValidaCodigoValidacao(string email, string codigo)
        {
            throw new NotImplementedException();
        }

        
        public Cliente Login(string email, string password)
        {
            var cliente = clienteDAO.GetClienteEmail(email);

            // return null if user not found
            if (cliente == null || !cliente.Password.Equals(HashPassword(password)))
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, cliente.IdCliente.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            cliente.Token = tokenHandler.WriteToken(token);

            return cliente;
        }


        public Cliente EditarEmail(string token, string novoEmail)
        {
            var cliente = clienteDAO.GetClienteToken(token);
            if(cliente == null)
            {
                return null;
            }
            cliente.Email = novoEmail;
            clienteDAO.EditarEmail(cliente);
            return cliente;
        }


        public Cliente EditarNome(string token, string novoNome)
        {
            var cliente = clienteDAO.GetClienteToken(token);
            if (cliente == null)
            {
                return null;
            }
            cliente.Nome = novoNome;
            clienteDAO.EditarNumTelemovel(cliente);
            return cliente;
        }


        public Cliente EditarPassword(string token, string novaPassword)
        {
            var cliente = clienteDAO.GetClienteToken(token);
            if (cliente == null)
            {
                return null;
            }
            cliente.Password = HashPassword(novaPassword);
            clienteDAO.EditarNumTelemovel(cliente);
            return cliente;
        }

        public Cliente EditarNumTelemovel(string token, int numTelemovel)
        {
            var cliente = clienteDAO.GetClienteToken(token);
            if (cliente == null)
            {
                return null;
            }
            cliente.NumTelemovel = numTelemovel;
            clienteDAO.EditarNumTelemovel(cliente);
            return cliente;
        }
    }
}