using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using API.Data;
using API.Entities;
using API.Helpers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace API.Business
{
    public interface IClienteService
    {
        IList<Erro> CriarConta(string nome, string email, string password, int numTelemovel);
        Tuple<Email, Email> GetEmails(string email);
        IList<Erro> ValidarConta(string email, string codigo);
        Tuple<IList<Erro>, string> Login(string email, string password);
        IList<Erro> EditarDados(int idCliente, string novoNome, string novoEmail, string novaPassword, int numTelemovel);
        Cliente GetCliente(int idCliente);
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



        public IList<Erro> CriarConta(string nome, string email, string password, int numTelemovel)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo.");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password","Campo não poder ser nulo.");
            }

            IList<Erro> erros = new List<Erro>();

            /*if (_clienteDAO.ExisteEmail(email))
            {
                erros.Add(new Erro{Codigo = 1, Mensagem = "O email inserido já existe." });

            }
            if (_clienteDAO.ExisteNumTelemovel(numTelemovel))
            {
                erros.Add(new Erro { Codigo = 2, Mensagem = "O número de telemóvel inserido já existe." });
            }*/
            if (!ValidaNome(nome))
            {
                erros.Add(new Erro { Codigo = 3, Mensagem = "O nome inserido ultrapassa o limite de caractéres." });
            }
            if (!ValidaEmail(email))
            {
                erros.Add(new Erro { Codigo = 4, Mensagem = "O formato do email inserido é inválido." });
            }
            if (!ValidaPassword(password))
            {
                erros.Add(new Erro { Codigo = 5, Mensagem = "A password deve ter entre 8 e 45 caractéres." });
            }
            if (!ValidaNumTelemovel(numTelemovel))
            {
                erros.Add(new Erro { Codigo = 6, Mensagem = "O número de telemóvel inserido é inválido." });
            }

            if (!erros.Any()) { 
                string codigoValidacao = GerarCodigo();
                Cliente cliente = new Cliente { Nome = nome, Email = email, NumTelemovel = numTelemovel };
                cliente.Password = HashPassword(password);
                _clienteDAO.InserirCliente(cliente, codigoValidacao);
            }
            return erros;
        }


        public Tuple<Email, Email> GetEmails(string email)
        {
            string codigoValidacao = "";//_clienteDAO.GetCodigoValidacao(email);

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

       public IList<Erro> ValidarConta(string email, string codigo)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo!");
            }
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentNullException("Codigo", "Campo não poder ser nulo!");
            }

            IList<Erro> erros = new List<Erro>();

            /*if (!_clienteDAO.ExisteEmail(email))
            {
                erros.Add(new Erro { Codigo = 7, Mensagem = "O Email inserido não existe!" });
            }
            if (_clienteDAO.ContaValida(email)) 
            {
                erros.Add(new Erro { Codigo = 11, Mensagem = "A conta já se encontra validada." });
            }
            if (!_clienteDAO.GetCodigoValidacao(email).Equals(codigo))
            {
                int numTentativas = _clienteDAO.GetNumTentativas(email)+1;
                int numMaximoTentativas = _appSettings.NumTentativasCodigoValidacao;
                if (numTentativas <= numMaximoTentativas)
                {
                    erros.Add(new Erro { Codigo = 8, Mensagem = "Codigo de Validação invlálido .Tem mais " + (numMaximoTentativas-numTentativas) + "tentativas." });
                    _clienteDAO.IncrementaNumTentativas(email);
                }
                else
                {
                    erros.Add(new Erro { Codigo = 9, Mensagem = "Numero de tentativas excedido. A sua conta foi removida." });
                    _clienteDAO.RemoverContaInvalida(email);
                }
            }

            if (!erros.Any())
            {
                _clienteDAO.ValidarConta(email);
            }*/
            return erros;
        }



        public Tuple<IList<Erro>,string> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "FieldNull");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("Password", "FieldNull");
            }

            IList<Erro> erros = new List<Erro>();

            /*if (!_clienteDAO.ContaValida(email))
            {
                erros.Add(new Erro { Codigo = 10, Mensagem = "A conta ainda não se encontra verificada." });
            }*/

            string resultToken = null;
            if (!erros.Any())
            {
                /*Cliente cliente = _clienteDAO.GetClienteEmail(email);
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
                    resultToken = tokenHandler.WriteToken(token);
                }
                else
                {
                    erros.Add(new Erro { Codigo = 12, Mensagem = "Email ou Password Incorreta." });
                }*/
            }
            return new Tuple<IList<Erro>,string>( erros,resultToken);
        }



        public IList<Erro> EditarDados(int idCliente, string novoNome, string novoEmail, string novaPassword, int numTelemovel)
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

            IList<Erro> erros = new List<Erro>();

            /*if (_clienteDAO.ExisteEmail(novoEmail))
            {
                erros.Add(new Erro { Codigo = 1, Mensagem = "O email inserido já existe." });
            }

            if (_clienteDAO.ExisteNumTelemovel(numTelemovel))
            {
                erros.Add(new Erro { Codigo = 2, Mensagem = "O número de telemóvel inserido já existe." });
            }*/
            if (!ValidaNome(novoNome))
            {
                erros.Add(new Erro { Codigo = 3, Mensagem = "O nome inserido ultrapassa o limite de caractéres." });
            }
            if (!ValidaEmail(novoEmail))
            {
                erros.Add(new Erro { Codigo = 4, Mensagem = "O formato do email inserido é inválido." });
            }
            if (!ValidaPassword(novaPassword))
            {
                erros.Add(new Erro { Codigo = 5, Mensagem = "A password deve ter entre 8 e 45 caractéres." });
            }
            if (!ValidaNumTelemovel(numTelemovel))
            {
                erros.Add(new Erro { Codigo = 6, Mensagem = "O número de telemóvel inserido é inválido." });
            }


            if (!erros.Any())
            {
                /*Cliente cliente = _clienteDAO.GetClienteId(idCliente);

                cliente.Nome = novoNome;
                cliente.Email = novoEmail;
                cliente.Password = HashPassword(novaPassword);
                cliente.NumTelemovel = numTelemovel;

                _clienteDAO.EditarConta(cliente);*/
            }
            return erros;
        }


        public Cliente GetCliente(int idCliente)
        {
            return null;
            //return _clienteDAO.GetClienteId(idCliente);
        }

    }
}