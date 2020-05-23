using System;
namespace API.Services.Pagamentos.MBWay
{
    public class MBWayService : IPagamentosService
    {
        public bool Pagar(PagamentoModel model)
        {
            return true;
        }
    }
}
