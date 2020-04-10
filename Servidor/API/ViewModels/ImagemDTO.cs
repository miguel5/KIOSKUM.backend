using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace API.ViewModels
{
    public class ImagemDTO
    {
        [Required]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
}
