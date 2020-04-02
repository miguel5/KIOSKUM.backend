namespace API.Helpers
{
    
    public class AppSettings
    {
        public string Secret { get; set; }

        public EmailSettings EmailSettings { get; set; }

        public DBSettings DBSettings { get; set; }

        public ImageSettings ImageSettings { get; set; }

        public int NumTentativasCodigoValidacao { get; set; }
    }
}
