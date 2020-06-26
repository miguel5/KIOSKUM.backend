using System;
using System.Collections.Generic;
using System.Linq;
using DTO;

namespace Services.Pagamentos.MBWay
{
    public class MBWayService : IPagamentosService
    {
        public ServiceResult<string> PedirPagamento(PagamentoModel model)
        {
            IList<int> erros = new List<int>();
            return new ServiceResult<string> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = Guid.NewGuid().ToString() };
        }
    }
}