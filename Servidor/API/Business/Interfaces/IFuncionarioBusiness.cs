using System;
using API.Entities;
using API.ViewModels.FuncionarioDTOs;

namespace API.Business.Interfaces
{
    public interface IFuncionarioBusiness
    {
        ServiceResult CriarConta(FuncionarioViewDTO model);
        ServiceResult EditarConta(FuncionarioViewDTO model);
        ServiceResult<FuncionarioViewDTO> GetFuncionario(int numFuncionario);
    }
}
