using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ReservaController : ControllerBase
    {
        private List<Reserva> reservas;
        private readonly ILogger<ReservaController> logger;

        public ReservaController(ILogger<ReservaController> logger)
        {
            this.logger = logger;
            reservas = new List<Reserva>();
            Reserva r;
            for (int i = 0; i < 5; i++)
            {
                r = new Reserva(1,new List<Tuple<int, int, string>> { new Tuple<int, int, string>(1, 2, ""),new Tuple<int, int, string>(3, 1, "Sem sal.")}, 5.70, DateTime.Now);
                r.AlteraEstadoReserva(1, 'a');
                r.RegistaPagamento(DateTime.Now);
                r.AlteraEstadoReserva(2, 'e');
                reservas.Add(r);
            }
        }
       

        [HttpGet]
        [Route("Todos")]
        public IList<Reserva> Get()
        {
            return reservas;

        }

    }

}
