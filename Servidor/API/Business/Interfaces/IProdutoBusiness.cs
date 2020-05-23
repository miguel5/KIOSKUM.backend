using System;
using System.Collections.Generic;
using API.Entities;
using API.ViewModels.ProdutoDTOs;

namespace API.Business.Interfaces
{
    public interface IProdutoBusiness
    {
        ServiceResult<Tuple<string, string>> RegistarProduto(RegistarProdutoDTO model, string extensao);
        ServiceResult<Tuple<string, string>> EditarProduto(EditarProdutoDTO model, string extensao);
        IList<ProdutoViewDTO> GetProdutosDesativados();
        ServiceResult<ProdutoViewDTO> GetProduto(int idProduto);
        ServiceResult DesativarProduto(int idProduto);
        ServiceResult AtivarProduto(int idProduto);
    }
}