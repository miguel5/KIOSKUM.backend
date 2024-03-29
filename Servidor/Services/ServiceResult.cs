﻿using DTO;

namespace Services
{
    public class ServiceResult<TResult>
    {
        public ErrosDTO Erros { get; set; }
        public bool Sucesso { get; set; }
        public TResult Resultado { get; set; }
    }

    public class ServiceResult
    {
        public ErrosDTO Erros { get; set; }
        public bool Sucesso { get; set; }
    }
}