using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace API.Models
{
    public class GestorDados
    {
        public Dictionary<string, int> Tentativas;
        public Dictionary<string, string> Codigos;
        public Dictionary<string, Tuple<string, string, int>> Dados;

        private readonly string meuEmail = "espeta_espeta@portugalmail.pt";
        public readonly string meuPassword = "arroz_doce";


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


        private void EnviarEmail(string email, string titulo, string conteudo)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress(meuEmail);
            mail.Subject = titulo;
            mail.Body = conteudo;

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.portugalmail.pt"; //Or Your SMTP Server Address
            smtp.Credentials = new System.Net.NetworkCredential
                 ("espeta_espeta@portugalmail.pt", "meteantonio"); // ***use valid credentials***
            smtp.Port = 587;

            //Or your Smtp Email ID and Password
            smtp.EnableSsl = false;
            smtp.Send(mail);
        }


        public void CriarConta(string nome, string email, string password, int numTelemovel)
        {
            string titulo = "Boas Vindas ao KIOSK UM!";
            string conteudo = File.ReadAllText("/Users/lazaropinheiro/Desktop/C#/Email/Email/BoasVindas.txt");
            EnviarEmail(email, titulo, conteudo);

            titulo = "Código de confirmação do seu e-mail no KIOSK UM";
            string codigo = GerarCodigo();
            conteudo = "Para confirmar o seu e-mail, insira o código de 8 dígitos no KIOSK UM: " + codigo;
            EnviarEmail(email, titulo, conteudo);


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
