using System;
using System.Collections.Generic;
using API.Entities;
using API.ViewModels.CategoriaDTOs;
using API.ViewModels.ProdutoDTOs;

namespace API.Business.Interfaces
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
