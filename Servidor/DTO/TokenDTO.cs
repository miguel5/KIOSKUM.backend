using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class TokenDTO
    {
        [Required]
        public string Token { get; set; }
    }
}