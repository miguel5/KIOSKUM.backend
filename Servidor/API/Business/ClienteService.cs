using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using API.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using API.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using API.Helpers;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API.Business
{
    public interface IClienteService
    {
        Task<bool> CriarConta(string nome, string email, string password, int numTelemovel);
        bool ValidaCodigoValidacao(string email, string codigo);
        Cliente Login(string Email, string Password);
        bool EditarEmail(string token, string novoEmail);
        bool EditarNome(string token, string novoNome);
        bool EditarPassword(string token, string novaPassword);
        bool EditarNumTelemovel(string token, int numTelemovel);
        IList<Reserva> GetHistoricoReservas(string token);
    }


    public class ClienteService : IClienteService
    {
        private readonly AppSettings _appSettings;
        private ClienteDAO clienteDAO;
        private List<Cliente> _clientes = new List<Cliente>
        {
            new Cliente { IdCliente = 1, Nome = "Lázaro", Email = "lazaro.pinheiro1998@gmail.com", Password = "123456", NumTelemovel = 913136226, Token = ""}
        };

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
            bool sucesso = true;
            if (string.IsNullOrWhiteSpace(Nome))
            {
                throw new ArgumentNullException("Nome","Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentNullException("Email", "Parametro não pode ser nulo");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                throw new ArgumentNullException("Password", "Parametro não pode ser nulo");
            }

            if(!ValidaEmail(Email) && !ValidaPassword(Password) && ValidaNumTelemovel(NumTelemovel) &&
                !clienteDAO.existeEmail(Email) && !clienteDAO.existeNumTelemovel(NumTelemovel))
            {
                sucesso = false;
            }
            else
            {
                string codigoValidacao = GerarCodigo();

                /*Tentativas.Add(email, 0);
                Codigos.Add(email, codigo);
                Dados.Add(email, new Tuple<string, string, int>(nome, password, numTelemovel));*/

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
            var cliente = _clientes.SingleOrDefault(x => x.Email == email && x.Password == password);

            // return null if user not found
            if (cliente == null)
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


        public bool EditarEmail(string token, string novoEmail)
        {
            throw new NotImplementedException();
        }


        public bool EditarNome(string token, string novoNome)
        {
            throw new NotImplementedException();
        }


        public bool EditarPassword(string token, string novaPassword)
        {
            throw new NotImplementedException();
        }

        public bool EditarNumTelemovel(string token, int numTelemovel)
        {
            throw new NotImplementedException();
        }


        public IList<Reserva> GetHistoricoReservas(string token)
        {
            throw new NotImplementedException();
        }
    }
}
