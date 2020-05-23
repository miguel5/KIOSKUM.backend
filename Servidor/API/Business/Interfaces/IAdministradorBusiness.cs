using System;
using API.Entities;
using API.ViewModels;

namespace API.Business.Interfaces
{
    public interface IAdministradorBusiness
    {
        ServiceResult CriarConta(AdministradorDTO model);
        ServiceResult<TokenDTO> Login(AutenticacaoDTO model);
        ServiceResult EditarDados(int idFuncionario, EditarAdministradorDTO model);
        ServiceResult<AdministradorDTO> GetAdministrador(int idAdministrador);
    }
}
