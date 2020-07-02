using System;
using System.Collections.Generic;
using Entities;
using DTO.ProdutoDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IProdutoBusiness
    {
        ServiceResult<Tuple<string, string>> RegistarProduto(RegistarProdutoDTO model, string extensao);
        ServiceResult<Tuple<string, string>> EditarProduto(EditarProdutoDTO model, string extensao);
        ServiceResult<ProdutoViewDTO> GetProduto(int idProduto);
        ServiceResult DesativarProduto(int idProduto);
        ServiceResult AtivarProduto(int idProduto);
    }
}