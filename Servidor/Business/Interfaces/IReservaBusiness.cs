using System.Collections.Generic;
using Entities;
using DTO.ReservaDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IReservaBusiness
    {
        ServiceResult RegistarReserva(int idCliente, RegistarReservaDTO model);
        ServiceResult FuncionarioDecideReserva(FuncionarioDecideReservaDTO model, bool decisao);
        IList<ReservaViewDTO> GetReservasEstado(EstadosReservaEnum estadosReserva);
        ServiceResult EntregarReserva(EntregarReservaDTO model);
    }
}
