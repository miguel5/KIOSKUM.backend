using DTO;
using DTO.AdministradorDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IAdministradorBusiness
    {
        ServiceResult CriarConta(AdministradorViewDTO model);
        ServiceResult<TokenDTO> Login(AutenticacaoDTO model);
        ServiceResult EditarConta(int idFuncionario, EditarAdministradorDTO model);
        ServiceResult<AdministradorViewDTO> GetAdministrador(int idAdministrador);
    }
}
