using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class EditarNomeModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Nome { get; set; }
    }
}
