using System;
using System.Collections.Generic;
using DAO;
using DAO.Interfaces;
using Entities;
using Helpers;
using Microsoft.Extensions.Options;

namespace Inquiry_MBWAY_API
{
    class Program
    {
        private readonly IReservaDAO _reservaDAO;
        private readonly AppSettings _appSettings;

        public Program(IReservaDAO reservaDAO, IOptions<AppSettings> appSettings)
        {
            _reservaDAO = reservaDAO;
            _appSettings = appSettings.Value;
        }

        public static void Main()
        {
            //IList<Reserva> listaReservasAceites = _reservaDAO.GetReservasEstado((int)EstadosReservaEnum.Aceite);


            /*
             * Codigo API MBWAY
             * Inicio
             */
            /*Random random = new Random();
            foreach (Reserva reserva in listaReservasAceites)
            {
                var x = random.NextDouble();
                if (x <= 0.03)
                {
                    reserva.Estado = EstadosReservaEnum.Cancelada;
                    _reservaDAO.EditarReserva(reserva);
                }
                else if (x > 0.12)
                {
                    reserva.Estado = EstadosReservaEnum.Aceite;
                    _reservaDAO.EditarReserva(reserva);
                }
                else { }

            }*/

            /*
             * Fim
             */
        }
    }
}
