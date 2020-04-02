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
        [DataType(DataType.Upload)]
        //[FileExtensions(Extensions = "jpg,png,jpeg,bmp")]
        public IFormFile File { get; set; }
    }
}
