using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace API.ViewModels
{
    public class ImagemDTO
    {
        [Required]
        public int IdProduto { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
