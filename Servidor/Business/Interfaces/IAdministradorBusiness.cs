using DTO;
using DTO.TrabalhadorDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IAdministradorBusiness
    {
        ServiceResult CriarConta(TrabalhadorViewDTO model);
        ServiceResult<TokenDTO> Login(AutenticacaoTrabalhadorDTO model);
        ServiceResult EditarConta(int idFuncionario, EditarTrabalhadorDTO model);
        ServiceResult<TrabalhadorViewDTO> GetAdministrador(int idFuncionario);
    }
}