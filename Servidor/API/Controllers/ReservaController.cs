using System;
using System.Collections.Generic;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    
    [ApiController]
    [Route("api/reserva")]
    public class ReservaController : ControllerBase
    {
        /*private List<Reserva> reservas;
        private readonly ILogger<ReservaController> _logger;

        public ReservaController(ILogger<ReservaController> logger)
        {
            _logger = logger;
            reservas = new List<Reserva>();
            Reserva r;
            for (int i = 0; i < 5; i++)
            {
                r = new Reserva();
                r.IdReserva = i;
                r.IdCliente = 1;
                r.Items = new List<Tuple<int, int, string>> { new Tuple<int, int, string>(1, 2, "") ,  new Tuple<int, int, string>(3, 1, "Sem sal.") };
                r.Preco = 5.70;
                r.HoraEntrega = DateTime.Now;
            }
        }
       

        [HttpGet]
        [Route("todos")]
        public IList<Reserva> Get()
        {
            return reservas;

        }*/

    }

}
