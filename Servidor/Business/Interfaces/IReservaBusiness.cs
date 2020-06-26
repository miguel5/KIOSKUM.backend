using System.Collections.Generic;
using Entities;
using DTO.ReservaDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IReservaBusiness
    {
        ServiceResult RegistarReserva(int idCliente, RegistarReservaDTO model);
        ServiceResult FuncionarioDecideReserva(int idFuncionario, int idReserva, bool decisao);
        IList<ReservaViewDTO> GetReservasEstado(EstadosReservaEnum estadosReserva);
        ServiceResult EntregarReserva(int idFuncionario, int idReserva);
    }
}