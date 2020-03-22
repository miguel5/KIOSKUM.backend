using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using API.Business;
using API.Data;
using System.Text.RegularExpressions;

namespace API.Models
{
    public class ClienteService
    {
        private ClienteDAO clienteDAO;
        /*public Dictionary<string, int> Tentativas;
        public Dictionary<string, string> Codigos;
        public Dictionary<string, Tuple<string, string, int>> Dados;*/

        public ClienteService()
        {
            clienteDAO = new ClienteDAO();
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

                EmailSender emailSender = new EmailSender();
                await emailSender.SendEmail(Email, emailGerarCodigo);
                await emailSender.SendEmail(Email, emailBoasVindas);
            }
            return sucesso;
        }


        public bool ValidaCodigoValidacao(string Email, string Codigo)
        {
            bool resultado = false;
            /*if (Codigos[email].Equals(codigo))
            {
                resultado = true;
            }
            else
            {
                if (Tentativas[email] < 3)
                {
                    Tentativas[email]++;
                }
                else
                {
                    LimpaEntradaCliente(email);
                }
            }*/
            return resultado;
        }


        /*public bool Login(string Email, string Password)
        {
            bool sucesso;
            if (!clienteDAO.existeEmail(Email))
            {
                sucesso = false;
            }
            else
            {
                Cliente cliente = clienteDAO.getCliente(Email);
                sucesso = cliente.ComparaPasswords(Password);
            }
            return sucesso;
        }


        public bool EditarEmail(string Email, string NovoEmail)
        {
            bool sucesso;
            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentNullException("Email", "Parametro não pode ser nulo");
            }

            if (!ValidaEmail(Email) && !clienteDAO.existeEmail(Email))
            {
                sucesso = false;
            }
            else
            {
                clienteDAO.EditarNome(Email, NovoEmail);
                sucesso = true;
            }
            return sucesso;
        }


        public bool EditarNome(string Email, string NovoNome)
        {
            bool sucesso;
            if (string.IsNullOrWhiteSpace(NovoNome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }

            if (!clienteDAO.existeEmail(Email))
            {
                sucesso = false;
            }
            else
            {
                clienteDAO.EditarNome(Email, NovoNome);
                sucesso = true;
            }
            return sucesso;
        }


        public bool EditarPassword(string Email, string NovaPassword)
        {
            bool sucesso;
            if (string.IsNullOrWhiteSpace(NovaPassword))
            {
                throw new ArgumentNullException("Password", "Parametro não pode ser nulo");
            }

            if (!clienteDAO.existeEmail(Email))
            {
                sucesso = false;
            }
            else
            {
                clienteDAO.EditarNome(Email, NovaPassword);
                sucesso = true;
            }
            return sucesso;
        }


        public IList<Reserva> GetHistoricoReservas(string Email)
        {
            return null;
        }*/

    }
}
