﻿namespace Services.Pagamentos
{
    public interface IPagamentosService
    {
        ServiceResult<string> PedirPagamento(PagamentoModel model);
    }
}