using System;
namespace API.Services.Pagamentos
{
    public interface IPagamentosService
    {
        bool PedirPagamento(PagamentoModel model);
        bool Pagar(PagamentoModel model);
    }
}
