using API.Entities;
using API.ViewModels;
using API.ViewModels.AdministradorDTOs;

namespace API.Business.Interfaces
{
    public interface IAdministradorBusiness
    {
        ServiceResult CriarConta(AdministradorViewDTO model);
        ServiceResult<TokenDTO> Login(AutenticacaoDTO model);
        ServiceResult EditarConta(int idFuncionario, EditarAdministradorDTO model);
        ServiceResult<AdministradorViewDTO> GetAdministrador(int idAdministrador);
    }
}
