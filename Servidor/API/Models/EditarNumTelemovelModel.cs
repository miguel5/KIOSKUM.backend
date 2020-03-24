using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class EditarNumTelemovelModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public int NumTelemovel { get; set; }
    }
}
