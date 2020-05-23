using System;
namespace API.Services.Pagamentos
{
    public interface IPagamentosService
    {
        bool Pagar(PagamentoModel model);
    }
}
