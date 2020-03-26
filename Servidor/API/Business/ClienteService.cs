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

namespace API.Business
{
    public interface IClienteService
    {
        Task<bool> CriarConta(string nome, string email, string password, int numTelemovel);
        bool ValidaCodigoValidacao(string email, string codigo);
        Cliente Login(string Email, string Password);
        Cliente EditarDados(string token, string novoNome, string novoEmail, string novaPassword, int numTelemovels);
    }


    public class ClienteService : IClienteService
    {
        private readonly AppSettings _appSettings;
        Cliente c;
        private ClienteDAO clienteDAO;

        public ClienteService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            clienteDAO = new ClienteDAO();
            c = new Cliente { IdCliente = 1, Nome = "Lázaro", Email = "lazaro.pinhairo1998@gmail.com", NumTelemovel = 913136226 };
            c.Password = HashPassword("1234567890");
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


        private bool ValidaEmail(string email)
        {
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            return rx.IsMatch(email);
        }

        private bool ValidaPassword(string password)
        {
            return password.Length >= 8;
        }

        private bool ValidaNumTelemovel(int numTelemovel)
        {
            Regex rx = new Regex("^9[1236]{1}[0-9]{7}$");
            return rx.IsMatch(numTelemovel.ToString());
        }


        public async Task<bool> CriarConta(string nome, string email, string password, int numTelemovel)
        {
            bool sucesso = false;
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password", "Parametro não pode ser nulo");
            }

            if (ValidaEmail(email) && ValidaPassword(password) && ValidaNumTelemovel(numTelemovel)) //&&
                //!clienteDAO.ExisteEmail(email) && !clienteDAO.ExisteNumTelemovel(numTelemovel))
            {
                string codigoValidacao = GerarCodigo();
                Cliente cliente = new Cliente { Nome = nome, Email = email, Password = password, NumTelemovel = numTelemovel };
                //clienteDAO.InserirCliente(cliente, codigoValidacao);

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
                await emailSender.SendEmail(email, emailGerarCodigo);
                await emailSender.SendEmail(email, emailBoasVindas);

                sucesso = true;
            }
            return sucesso;
        }


        public bool ValidaCodigoValidacao(string email, string codigo)
        {
            throw new NotImplementedException();
        }


        public Cliente Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password", "Parametro não pode ser nulo");
            }

            var cliente = c;//clienteDAO.GetClienteEmail(email);

            if (!c.Email.Equals(email))
            {
                return null;
            }

            // return null if user not found
            if (cliente == null || cliente.Password.Equals(HashPassword(password)) == false)
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
            //clienteDAO.EditarDados(cliente);
            return cliente;
        }


        public Cliente EditarDados(string token, string novoNome, string novoEmail, string novaPassword, int numTelemovel)
        {
            if (string.IsNullOrWhiteSpace(novoEmail))
            {
                throw new ArgumentNullException("Email", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(novoNome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(novaPassword))
            {
                throw new ArgumentNullException("Password", "Parametro não pode ser nulo");
            }

            Cliente cliente = c;//clienteDAO.GetClienteToken(token);
            if (cliente != null && ValidaEmail(novoEmail) && ValidaPassword(novaPassword) && ValidaNumTelemovel(numTelemovel))
            {
                cliente.Nome = novoNome;
                cliente.Email = novoEmail;
                cliente.Password = HashPassword(novaPassword);
                cliente.NumTelemovel = numTelemovel;

                //clienteDAO.EditarDados(cliente);
            }
            else
            {
                cliente = null;
            }
            return cliente;
        }
    }
}