using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class EditarPasswordModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string Password { get; set; }

    }
}