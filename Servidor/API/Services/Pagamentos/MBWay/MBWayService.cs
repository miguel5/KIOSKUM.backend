using System;
using System.Collections.Generic;
using System.Linq;
using API.Entities;
using API.ViewModels;

namespace API.Services.Pagamentos.MBWay
{
    public class MBWayService : IPagamentosService
    {
        public bool Pagar(PagamentoModel model)
        {
            return true;
        }

        public ServiceResult<string> PedirPagamento(PagamentoModel model)
        {
            IList<int> erros = new List<int>();
            return new ServiceResult<string> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = Guid.NewGuid().ToString() };
        }
    }
}
