using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace API.ViewModels
{
    public class CategoriaDTO
    {
        [Required]
        public string Nome { get; set; }

        public Url Url { get; set; }
    }
}
