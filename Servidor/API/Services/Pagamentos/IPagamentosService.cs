using System;
using API.Entities;

namespace API.Services.Pagamentos
{
    public interface IPagamentosService
    {
        ServiceResult<string> PedirPagamento(PagamentoModel model);
        bool Pagar(PagamentoModel model);
    }
}
