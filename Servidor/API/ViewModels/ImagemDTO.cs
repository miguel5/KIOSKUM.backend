using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace API.ViewModels
{
    public class ImagemDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
}
