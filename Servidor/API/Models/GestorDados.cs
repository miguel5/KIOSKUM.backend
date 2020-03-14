using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Newtonsoft.Json;

namespace API.Models
{
    public class GestorDados
    {
        public Dictionary<string, int> Tentativas;
        public Dictionary<string, string> Codigos;
        public Dictionary<string, Tuple<string, string, int>> Dados;

        private readonly string myEmail = "espeta_espeta@portugalmail.pt";
        public readonly string myPassword = "meteantonio";
        public readonly string serverAdressSMTP = "smtp.portugalmail.pt";

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


        private void EnviarEmail(string email, Email mensagem)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress(myEmail);
            mail.Subject = mensagem.Assunto;
            mail.Body = mensagem.Conteudo;

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = serverAdressSMTP; //Or Your SMTP Server Address
            smtp.Credentials = new System.Net.NetworkCredential
                 (myEmail, myPassword); // ***use valid credentials***
            smtp.Port = 587;

            //Or your Smtp Email ID and Password
            smtp.EnableSsl = false;
            smtp.Send(mail);
        }


        public void CriarConta(string nome, string email, string password, int numTelemovel)
        {
            string pathEmailBoasVindas = "D:\\home\\site\\wwwroot\\Files\\EmailBoasVindas.json";
            StreamReader sr = new StreamReader(pathEmailBoasVindas);
            string json = sr.ReadToEnd();
            Email emailBoasVindas = JsonConvert.DeserializeObject<Email>(json);
            EnviarEmail(email, emailBoasVindas);

            
            string codigo = GerarCodigo();
            
            string pathEmailgerarCodigo = "D:\\home\\site\\wwwroot\\Files\\EmailGerarCodigo.json";
            sr = new StreamReader(pathEmailgerarCodigo);
            json = sr.ReadToEnd();
            Email emailGerarCodigo = JsonConvert.DeserializeObject<Email>(json);
            emailGerarCodigo.AdcionaCodigo(codigo);
            EnviarEmail(email, emailGerarCodigo);

            Tentativas.Add(email, 0);
            Codigos.Add(email, codigo);
            Dados.Add(email, new Tuple<string, string, int>(nome, password, numTelemovel));
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
