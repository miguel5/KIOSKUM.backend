﻿using System.ComponentModel.DataAnnotations;
using DTO.ProdutoDTOs;

namespace DTO.ReservaDTOs
{
    public class ItemViewDTO
    {
        [Required]
        public ProdutoViewDTO ProdutoView { get; set; }

        [Required]
        public int Quantidade { get; set; }

        [Required]
        public string Observacoes { get; set; }
    }
}