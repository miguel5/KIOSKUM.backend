using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class TokenDTO
    {
        [Required]
        public string Token { get; set; }
    }
}
