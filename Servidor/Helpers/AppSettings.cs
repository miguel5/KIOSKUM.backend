namespace Helpers
{
    
    public class AppSettings
    {
        public string Secret { get; set; }

        public string ServerUrl { get; set; }

        public EmailSettings EmailSettings { get; set; }

        public DBSettings DBSettings { get; set; }

        public int NumTentativasCodigoValidacao { get; set; }

        public BarSettings BarSettings { get; set; }
    }
}
