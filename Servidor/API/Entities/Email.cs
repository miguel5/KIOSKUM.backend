namespace API.Entities
{
    public class Email
    { 
        public string Assunto { get; set; }
        public string Conteudo { get; set; }


        public void AdcionaCodigo(string codigo)
        {
            Conteudo += codigo;
        }
    }
}