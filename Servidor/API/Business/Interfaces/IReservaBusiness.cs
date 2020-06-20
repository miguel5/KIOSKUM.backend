using System.Collections.Generic;
using API.Entities;
using API.ViewModels.ReservaDTOs;

namespace API.Business.Interfaces
{
    public interface IReservaBusiness
    {
        ServiceResult RegistarReserva(int idCliente, RegistarReservaDTO model);
        ServiceResult FuncionarioDecideReserva(FuncionarioDecideReservaDTO model, bool decisao);
        IList<ReservaViewDTO> GetReservasEstado(EstadosReservaEnum estadosReserva);
        ServiceResult EntregarReserva(EntregarReservaDTO model);
    }
}
