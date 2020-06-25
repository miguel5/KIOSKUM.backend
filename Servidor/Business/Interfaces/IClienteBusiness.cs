using Entities;
using DTO;
using DTO.ClienteDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IClienteBusiness
    {
        ServiceResult CriarConta(ClienteViewDTO model);
        ServiceResult<Email> GetEmailCodigoValidacao(string email, string webRootPath);
        ServiceResult ConfirmarConta(ConfirmarClienteDTO model);
        ServiceResult<Email> GetEmailBoasVindas(string email, string webRootPath);
        ServiceResult<TokenDTO> Login(AutenticacaoDTO model);
        ServiceResult EditarConta(int idCliente, EditarClienteDTO model);
        ServiceResult<ClienteViewDTO> GetCliente(int idCliente);
    }
}
