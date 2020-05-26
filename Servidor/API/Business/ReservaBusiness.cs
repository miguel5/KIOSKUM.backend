using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using API.Business.Interfaces;
using API.Data.Interfaces;
using API.Entities;
using API.Helpers;
using API.ViewModels;
using API.ViewModels.ReservaDTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Business
{
    public class ReservaBusiness : IReservaBusiness
    {
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IClienteDAO _clienteDAO;
        private readonly IProdutoDAO _produtoDAO;
        private readonly IReservaDAO _reservaDAO;

        public ReservaBusiness(ILogger<ReservaBusiness> logger, IOptions<AppSettings> appSettings, IMapper mapper, IClienteDAO clienteDAO, IProdutoDAO produtoDAO, IReservaDAO reservaDAO)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _clienteDAO = clienteDAO;
            _produtoDAO = produtoDAO;
            _reservaDAO = reservaDAO;
        }


        private bool ValidaItens(IList<Item> items)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> ValidaItens]");
            bool result = true;
            foreach(Item item in items) if (result)
            { 
                result = item.Quantidade >= 1 && item.Observacoes.Length >= 0 && item.Observacoes.Length <= 300 && _produtoDAO.ExisteProduto(item.IdProduto);
            }
            return result;
        }


        private bool ValidaHoraEntrega(DateTime horaEntrega)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> ValidaHoraEntrega]");
            BarSettings barSettings = _appSettings.BarSettings;
            DateTime abertura;
            DateTime encerramento;
            bool sucessoAbertura = DateTime.TryParseExact(barSettings.HoraAbertura, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out abertura);
            bool sucessoEncerramento = DateTime.TryParseExact(barSettings.HoraEncerramento, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out encerramento);
            DateTime now = DateTime.Now;
            return sucessoAbertura && sucessoEncerramento && horaEntrega.Date == now.Date && horaEntrega.TimeOfDay >= abertura.TimeOfDay && horaEntrega.TimeOfDay <= encerramento.TimeOfDay && (horaEntrega-now).TotalMinutes >= barSettings.TempoAprovocaoReserva;
        }


        public ServiceResult RegistarReserva(int idCliente, RegistarReservaDTO model)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> RegistarReserva]");
            if (model.Itens == null)
            {
                throw new ArgumentNullException("Items", "Campo não poder ser nulo!");
            }
            if (model.HoraEntrega == null)
            {
                throw new ArgumentNullException("HoraEntrega", "Campo não poder ser nulo!");
            }

            IList<int> erros = new List<int>();

            if (!_clienteDAO.ExisteCliente(idCliente)){
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            if(!ValidaItens(model.Itens))
            {
                _logger.LogDebug("Existe um Item que é inválido.");
                erros.Add((int)ErrosEnumeration.ItensInvalidos);
            }
            if (!ValidaHoraEntrega(model.HoraEntrega))
            {
                _logger.LogDebug($"A Hora {model.HoraEntrega} é inválida.");
                erros.Add((int)ErrosEnumeration.HoraEntregaInvalida);
            }

            if (!erros.Any())
            {
                Reserva reserva = _mapper.Map<Reserva>(model);
                _reservaDAO.RegistarReserva(reserva);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }
    }
}
