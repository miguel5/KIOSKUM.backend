using System;
using System.Collections.Generic;
using Entities;
using DTO.CategoriaDTOs;
using DTO.ProdutoDTOs;
using Services;

namespace Business.Interfaces
{
    public interface ICategoriaBusiness
    {
        ServiceResult<Tuple<string, string>> RegistarCategoria(RegistarCategoriaDTO model, string extensao);
        ServiceResult<Tuple<string, string>> EditarCategoria(EditarCategoriaDTO model, string extensao);
        IList<CategoriaViewDTO> GetCategoriasDesativadas();
        IList<CategoriaViewDTO> GetCategorias();
        ServiceResult<IList<ProdutoViewDTO>> GetProdutosCategoria(int idCategoria);
        ServiceResult<CategoriaViewDTO> GetCategoria(int idCategoria);
        ServiceResult DesativarCategoria(int idCategoria);
        ServiceResult AtivarCategoria(int idCategoria);
    }
}
