using API.Entities;
using API.ViewModels;
using API.ViewModels.ClienteDTOs;

namespace API.Business.Interfaces
{
    public interface IClienteBusiness
    {
        ServiceResult CriarConta(ClienteViewDTO model);
        ServiceResult<Email> GetEmailCodigoValidacao(string email);
        ServiceResult ConfirmarConta(ConfirmarClienteDTO model);
        ServiceResult<Email> GetEmailBoasVindas(string email);
        ServiceResult<TokenDTO> Login(AutenticacaoDTO model);
        ServiceResult EditarConta(int idCliente, EditarClienteDTO model);
        ServiceResult<ClienteViewDTO> GetCliente(int idCliente);
    }
}
