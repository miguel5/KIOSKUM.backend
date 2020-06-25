using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class ErrosDTO
    {
        [Required]
        public IList<int> Erros { get; set; }
    }
}
