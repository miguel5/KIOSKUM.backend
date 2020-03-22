using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using API.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using API.Entities;

namespace API.Business
{
    public interface IClienteService
    {
        Task<bool> CriarConta(string nome, string email, string password, int numTelemovel);
        bool ValidaCodigoValidacao(string email, string codigo);
        bool Login(string Email, string Password);
        bool EditarEmail(string token, string novoEmail);
        bool EditarNome(string token, string novoNome);
        bool EditarPassword(string token, string novaPassword);
        bool EditarNumTelemovel(string token, int numTelemovel);
        IList<Reserva> GetHistoricoReservas(string token);
    }


    public class ClienteService
    { 
        private ClienteDAO clienteDAO;


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


        public bool Login(string email, string password)
        {
            throw new NotImplementedException();
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
