using DTO.FuncionarioDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IFuncionarioBusiness
    {
        ServiceResult CriarConta(FuncionarioViewDTO model);
        ServiceResult EditarConta(FuncionarioViewDTO model);
        ServiceResult<FuncionarioViewDTO> GetFuncionario(int numFuncionario);
    }
}
