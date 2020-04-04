﻿using System;
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
        ServiceResult CriarConta(ClienteDTO model);
        ServiceResult<Tuple<Email, Email>> GetEmails(string email);
        ServiceResult ConfirmarConta(ConfirmarClienteDTO model);
        ServiceResult<TokenDTO> Login(AutenticacaoDTO model);
        ServiceResult EditarDados(int idCliente, ClienteDTO model);
        ServiceResult<ClienteDTO> GetCliente(int idCliente);
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



        public ServiceResult CriarConta(ClienteDTO model)
        {
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
               erros.Add(Erros.EmailJaExiste);

            }
            if (_clienteDAO.ExisteNumTelemovel(model.NumTelemovel))
            {
                erros.Add(Erros.NumTelemovelJaExiste);
            }
            if (!ValidaNome(model.Nome))
            {
                erros.Add(Erros.NomeInvalido);
            }
            if (!ValidaEmail(model.Email))
            {
                erros.Add(Erros.EmailInvalido);
            }
            if (!ValidaPassword(model.Password))
            {
                erros.Add(Erros.PasswordInvalida);
            }
            if (!ValidaNumTelemovel(model.NumTelemovel))
            {
                erros.Add(Erros.NumTelemovelInvalido);
            }

            if (!erros.Any()) { 
                string codigoValidacao = GerarCodigo();
                int numMaximoTentativas = _appSettings.NumTentativasCodigoValidacao;
                Cliente cliente = new Cliente { Nome = model.Nome, Email = model.Email, NumTelemovel = model.NumTelemovel };
                cliente.Password = HashPassword(model.Password);
                _clienteDAO.InserirCliente(cliente, codigoValidacao, numMaximoTentativas);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult<Tuple<Email, Email>> GetEmails(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("Email", "Campo não poder ser nulo.");
            }

            IList<int> erros = new List<int>();
            Tuple<Email, Email> emails = null;
            string codigoValidacao = _clienteDAO.GetCodigoValidacao(email);

            if(codigoValidacao != null)
            {
                string pathEmailBoasVindas = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", "EmailBoasVindas.json");
                StreamReader sr = new StreamReader(pathEmailBoasVindas);
                string json = sr.ReadToEnd();
                Email emailBoasVindas = JsonConvert.DeserializeObject<Email>(json);

                string pathEmailgerarCodigo = Path.Combine(_webHostEnvironment.ContentRootPath, "Files", "EmailGerarCodigo.json");
                sr = new StreamReader(pathEmailgerarCodigo);
                json = sr.ReadToEnd();
                Email emailGerarCodigo = JsonConvert.DeserializeObject<Email>(json);
                emailGerarCodigo.AdcionaCodigo(codigoValidacao);


                emails = new Tuple<Email, Email>(emailBoasVindas, emailGerarCodigo);
            }
            else
            {
                erros.Add(Erros.EmailNaoExiste);
            }
            return new ServiceResult<Tuple<Email, Email>> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = emails };
        }

       public ServiceResult ConfirmarConta(ConfirmarClienteDTO model)
        {
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
                erros.Add(Erros.EmailNaoExiste);
            }
            else
            {
                if (_clienteDAO.ContaConfirmada(model.Email))
                {
                    erros.Add(Erros.ContaJaConfirmada);
                }
                if (!_clienteDAO.GetCodigoValidacao(model.Email).Equals(model.Codigo))
                {
                    int numTentativas = _clienteDAO.GetNumTentativas(model.Email);
                    if (numTentativas > 0)
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
                    _clienteDAO.ValidarConta(model.Email);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }



        public ServiceResult<TokenDTO> Login(AutenticacaoDTO model)
        {
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

            if (!_clienteDAO.ContaConfirmada(model.Email))
            {
                erros.Add(Erros.ContaNaoConfirmada);
            }
            /*if (!_clienteDAO.ContaAtiva(email))
            {
                erros.Add(Erros.ContaDesativada);
            }*/
            if (!erros.Any())
            {
                Cliente cliente = _clienteDAO.GetClienteEmail(model.Email);
                if (cliente != null && cliente.Password.Equals(HashPassword(model.Password)))
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

            ServiceResult<TokenDTO> result = new ServiceResult<TokenDTO>();
            result.Erros = new ErrosDTO { Erros = erros };
            result.Sucesso = !erros.Any();
            result.Resultado = resultToken;

            return result;
        }



        public ServiceResult EditarDados(int idCliente, ClienteDTO model)
        {
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

            IList<int> erros = new List<int>();
            Cliente cliente = _clienteDAO.GetClienteId(idCliente);
            if (cliente == null)
            {
                Console.WriteLine("Erro");
            }
            if (_clienteDAO.ExisteEmail(model.Email) && !model.Email.Equals(cliente.Email))
            {
                erros.Add(Erros.EmailJaExiste);
            }

            if (_clienteDAO.ExisteNumTelemovel(model.NumTelemovel) && model.NumTelemovel != cliente.NumTelemovel)
            {
                erros.Add(Erros.NumTelemovelJaExiste);
            }
            if (!ValidaNome(model.Nome))
            {
                erros.Add(Erros.NomeInvalido);
            }
            if (!ValidaEmail(model.Email))
            {
                erros.Add(Erros.EmailInvalido);
            }
            if (!ValidaPassword(model.Password))
            {
                erros.Add(Erros.PasswordInvalida);
            }
            if (!ValidaNumTelemovel(model.NumTelemovel))
            {
                erros.Add(Erros.NumTelemovelInvalido);
            }


            if (!erros.Any())
            { 
                cliente.Nome = model.Nome;
                cliente.Email = model.Email;
                cliente.Password = HashPassword(model.Password);
                cliente.NumTelemovel = model.NumTelemovel;

                _clienteDAO.EditarConta(cliente);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros}, Sucesso = !erros.Any() };
        }


        public ServiceResult<ClienteDTO> GetCliente(int idCliente)
        {

            IList<int> erros = new List<int>();
            ClienteDTO clienteDTO = null;

            Cliente cliente = _clienteDAO.GetClienteId(idCliente);
            if(cliente == null)
            {
                erros.Add(Erros.ClienteNaoExiste);
            }

            if (!erros.Any())
            {
                clienteDTO = new ClienteDTO { Nome = cliente.Nome, Email = cliente.Email, Password = cliente.Password, NumTelemovel = cliente.NumTelemovel }; 
            }

            return new ServiceResult<ClienteDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = erros.Any(), Resultado = clienteDTO };
        }


        public void DesativarConta(int idCliente)
        { 
            //ClienteDAO.DesativarConta(idCliente);
        }
    }
}