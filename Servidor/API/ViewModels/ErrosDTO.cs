using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class ErrosDTO
    {
        [Required]
        public IList<int> Erros { get; set; }
    }
}
