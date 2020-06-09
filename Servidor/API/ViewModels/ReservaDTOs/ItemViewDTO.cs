using System;
using System.ComponentModel.DataAnnotations;
using API.ViewModels.ProdutoDTOs;

namespace API.ViewModels.ReservaDTOs
{
    public class ItemViewDTO
    {
        [Required]
        public ProdutoViewDTO ProdutoView { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public string Descricao { get; set; }
    }
}
