using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using API.Business.Interfaces;
using API.Data.Interfaces;
using API.Entities;
using API.Helpers;
using API.Services.Pagamentos;
using API.Services.Pagamentos.MBWay;
using API.ViewModels;
using API.ViewModels.ProdutoDTOs;
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
        private readonly IFuncionarioDAO _funcionarioDAO;
        private readonly IProdutoDAO _produtoDAO;
        private readonly IReservaDAO _reservaDAO;
        private readonly IPagamentosService _pagamentoService;

        public ReservaBusiness(ILogger<ReservaBusiness> logger, IOptions<AppSettings> appSettings, IMapper mapper, IClienteDAO clienteDAO, IFuncionarioDAO funcionarioDAO, IProdutoDAO produtoDAO, IReservaDAO reservaDAO, IPagamentosService pagamentoService)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _clienteDAO = clienteDAO;
            _funcionarioDAO = funcionarioDAO;
            _produtoDAO = produtoDAO;
            _reservaDAO = reservaDAO;
            _pagamentoService = pagamentoService;
        }

        private bool ValidaItens(IList<Item> itens)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> ValidaItens]");
            bool result = true;
            foreach(Item item in itens) if (result)
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

        private double CalculaValorTotalReserva(IList<Item> itens)
        {
            double precoTotal = 0;
            foreach(Item item in itens)
            {
                Produto produto = _produtoDAO.GetProduto(item.IdProduto);
                precoTotal += produto.Preco * item.Quantidade;
            }
            return Math.Round(precoTotal,2);
        }


        public ServiceResult RegistarReserva(int idCliente, RegistarReservaDTO model)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> RegistarReserva]");
            if (model.Itens == null)
            {
                throw new ArgumentNullException("Itens", "Campo não poder ser nulo!");
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
                reserva.Preco = CalculaValorTotalReserva(model.Itens);
                _reservaDAO.RegistarReserva(reserva);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult FuncionarioDecideReserva(FuncionarioDecideReservaDTO model, bool decisao)
        {
            IList<int> erros = new List<int>();

            if (!_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                erros.Add((int)ErrosEnumeration.NumFuncionarioNaoExiste);
            }

            if (!_reservaDAO.ExisteReserva(model.IdReserva))
            {
                erros.Add((int)ErrosEnumeration.ReservaNaoExiste);
            }

            if (!erros.Any())
            {
                Reserva reserva = _reservaDAO.GetReserva(model.IdReserva);
                if(reserva.Estado == EstadosReservaEnum.Pendente)
                {
                    reserva.Estado = decisao == true ? EstadosReservaEnum.Aceite : EstadosReservaEnum.Rejeitada;
                    reserva.IdFuncionarioDecide = model.NumFuncionario;
                    if (decisao == true)
                    {
                        int numTelemovel = _clienteDAO.GetContaId(reserva.IdCliente).NumTelemovel;
                        ServiceResult<string> resultado = _pagamentoService.PedirPagamento(new MBWayPagamentoModel { NumTelemovel = numTelemovel, Valor = reserva.Preco });
                        if (!resultado.Sucesso)
                        {
                            reserva.TransactionUniqueId = resultado.Resultado;
                            erros.Add((int)ErrosEnumeration.ErroNoPedidoDePagamento);
                            _reservaDAO.EditarReserva(reserva);
                        }
                    }
                    else
                    {
                        _reservaDAO.EditarReserva(reserva);
                    }
                }
                else
                {
                    erros.Add((int)ErrosEnumeration.TransicaoEstadosReservaImpossivel);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public IList<ReservaViewDTO> GetReservasEstado(EstadosReservaEnum estadosReserva)
        {
            IList<ReservaViewDTO> reservasViewDTO = new List<ReservaViewDTO>();

            IList<Reserva> reservas = _reservaDAO.GetReservasEstado((int) estadosReserva);

            foreach(Reserva reserva in reservas)
            {
                IList <ItemViewDTO>  itensDTO = new List<ItemViewDTO>();

                foreach(Item item in reserva.Itens)
                {
                    ItemViewDTO itemViewDTO = _mapper.Map<ItemViewDTO>(item);
                    Produto produto = _produtoDAO.GetProduto(item.IdProduto);
                    itemViewDTO.ProdutoView = _mapper.Map<ProdutoViewDTO>(produto);
                    itensDTO.Add(itemViewDTO);
                }

                ReservaViewDTO reservaViewDTO = _mapper.Map<ReservaViewDTO>(reserva);
                reservaViewDTO.Itens = itensDTO;

                reservasViewDTO.Add(reservaViewDTO);
            }
            return reservasViewDTO;
        }



        public ServiceResult EntregarReserva(EntregarReservaDTO model)
        {
            IList<int> erros = new List<int>();

            if (!_reservaDAO.ExisteReserva(model.IdReserva))
            {
                erros.Add((int)ErrosEnumeration.ReservaNaoExiste);
            }

            if (!_funcionarioDAO.ExisteNumFuncionario(model.NumFuncionario))
            {
                erros.Add((int)ErrosEnumeration.NumFuncionarioNaoExiste);
            }

            if (!erros.Any())
            {
                Reserva reserva = _reservaDAO.GetReserva(model.IdReserva);
                if(reserva.Estado == EstadosReservaEnum.Paga)
                {
                    reserva.Estado = EstadosReservaEnum.Entregue;
                    _reservaDAO.EditarReserva(reserva);
                }
                else
                {
                    erros.Add((int)ErrosEnumeration.TransicaoEstadosReservaImpossivel);
                }

            }

            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }
    }
}
