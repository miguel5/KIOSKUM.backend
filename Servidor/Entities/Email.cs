namespace Entities
{
    public class Email
    { 
        public string Assunto { get; set; }
        public string Conteudo { get; set; }

        public void AdicionaCodigo(string codigo)
        {
            Conteudo += codigo;
        }
    }
}