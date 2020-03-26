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
        bool EditarDados(int token, string novoNome, string novoEmail, string novaPassword, int numTelemovels);
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


        private bool ValidaNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }
            return nome.Length <= 45;
        }


        private bool ValidaEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Parametro não pode ser nulo");
            }
            Regex rx = new Regex(".+@([a-z\\-_\\.]+)\\.[a-z]*");
            
            return rx.IsMatch(email) && email.Length <= 45 && clienteDAO.ExisteEmail(email);
        }

        private bool ValidaPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password", "Parametro não pode ser nulo");
            }
             
            return password.Length >= 8 && password.Length <= 45;
        }

        private bool ValidaNumTelemovel(int numTelemovel)
        {
            Regex rx = new Regex("^9[1236]{1}[0-9]{7}$");

            return rx.IsMatch(numTelemovel.ToString()) && clienteDAO.ExisteNumTelemovel(numTelemovel);
        }


        public async Task<bool> CriarConta(string nome, string email, string password, int numTelemovel)
        {
            bool sucesso = false;
            if (ValidaNome(nome) && ValidaEmail(email) && ValidaPassword(password) && ValidaNumTelemovel(numTelemovel))
            {
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

            Cliente cliente = clienteDAO.GetClienteEmail(email);

            
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
            bool sucesso = false;
            Cliente cliente = clienteDAO.GetClienteId(idCliente);
            if (cliente != null && ValidaNome(novoNome) && ValidaEmail(novoEmail) && ValidaPassword(novaPassword) && ValidaNumTelemovel(numTelemovel))
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

    }
}