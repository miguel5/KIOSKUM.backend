using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using API.Business;

namespace API.Models
{
    public class GestorDados
    {
        public Dictionary<string, int> Tentativas;
        public Dictionary<string, string> Codigos;
        public Dictionary<string, Tuple<string, string, int>> Dados;

        public GestorDados()
        {
            Tentativas = new Dictionary<string, int>();
            Codigos = new Dictionary<string, string>();
            Dados = new Dictionary<string, Tuple<string, string, int>>();
        }

        private string GerarCodigo()
        {
            Random random = new Random();
            const string carateres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(carateres, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public async Task CriarConta(string nome, string email, string password, int numTelemovel)
        {
            string codigo = GerarCodigo();

            Tentativas.Add(email, 0);
            Codigos.Add(email, codigo);
            Dados.Add(email, new Tuple<string, string, int>(nome, password, numTelemovel));

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
            emailGerarCodigo.AdcionaCodigo(codigo);

            EmailSender emailSender = new EmailSender();
            await emailSender.SendEmail(email,emailGerarCodigo);
            await emailSender.SendEmail(email, emailBoasVindas);
        }

        public bool ValidaCodigo(string email, string codigo)
        {
            bool resultado = false;
            if (Codigos[email].Equals(codigo))
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
            }
            return resultado;
        }

        public void LimpaEntradaCliente(string email)
        {
            Tentativas.Remove(email);
            Codigos.Remove(email);
            Dados.Remove(email);
        }
    }
}
