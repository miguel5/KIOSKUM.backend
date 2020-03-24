using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class EditarEmailModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
