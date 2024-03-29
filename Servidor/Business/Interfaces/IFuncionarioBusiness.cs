﻿using DTO;
using DTO.TrabalhadorDTOs;
using Services;

namespace Business.Interfaces
{
    public interface IFuncionarioBusiness
    {
        ServiceResult CriarConta(TrabalhadorViewDTO model);
        ServiceResult<TokenDTO> Login(AutenticacaoTrabalhadorDTO model);
        ServiceResult EditarConta(EditarTrabalhadorDTO model);
        ServiceResult<TrabalhadorViewDTO> GetFuncionario(int numFuncionario);
    }
}