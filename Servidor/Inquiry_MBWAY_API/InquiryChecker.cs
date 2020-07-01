using System;
using System.Collections.Generic;
using System.Linq;
using DAO;
using DAO.Interfaces;
using Entities;
using Helpers;
using Microsoft.Extensions.Options;

namespace Inquiry_MBWAY_API
{
    class InquiryChecker
    {
        private readonly AppSettings _appSettings;

        public InquiryChecker(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public static void Main()
        {
            IReservaDAO _reservaDAO = new ReservaDAO();

            IList<Reserva> listaReservasAceites =_reservaDAO.GetReservasEstado((int)EstadosReservaEnum.Aceite);


            int y = _appSettings.MBWaySettings.MaxNumTokensPerInquiryRequest;
            var reservasAceitesSeparacao = listaReservasAceites.Select((x, i) => new { x, i }).GroupBy(i => i.i / 50, x => x.x);
            Random random = new Random();

            foreach (var reservasAceites in reservasAceitesSeparacao)
            {
                /*
                 * Codigo API MBWAY
                 * Inicio
                 */
                foreach(Reserva reserva in reservasAceites)
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
                    else{ }
                }

                /*
                 * Fim
                 */

            }
            
        }
    }
}
