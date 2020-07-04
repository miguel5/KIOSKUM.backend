using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using Business.Interfaces;
using DAO.Interfaces;
using DTO;
using DTO.ProdutoDTOs;
using DTO.ReservaDTOs;
using Entities;
using Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services;
using Services.Pagamentos;
using Services.Pagamentos.MBWay;

namespace Business
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
                _logger.LogWarning($"Não existe nenhum Cliente com o IdCliente {idCliente}.");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }

            if(!ValidaItens(model.Itens))
            {
                _logger.LogDebug("Existe um Item que é inválido.");
                erros.Add((int)ErrosEnumeration.ItensInvalidos);
            }
            /*if (!ValidaHoraEntrega(model.HoraEntrega))
            {
                _logger.LogDebug($"A Hora {model.HoraEntrega} é inválida.");
                erros.Add((int)ErrosEnumeration.HoraEntregaInvalida);
            }*/
            if (!erros.Any())
            {
                Reserva reserva = _mapper.Map<Reserva>(model);
                reserva.IdCliente = idCliente;
                reserva.Preco = CalculaValorTotalReserva(model.Itens);
                _reservaDAO.RegistarReserva(reserva);
            }

            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public ServiceResult FuncionarioDecideReserva(int idFuncionario, int idReserva, bool decisao)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> EditarReserva]");

            IList<int> erros = new List<int>();

            if (!_funcionarioDAO.ExisteIdFuncionario(idFuncionario))
            {
                _logger.LogWarning($"Não existe nenhum Funcionário com o IdFuncionario {idFuncionario}.");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }

            if (!_reservaDAO.ExisteReserva(idReserva))
            {
                _logger.LogWarning($"Não existe nenhuma Reserva com o IdReserva {idReserva}.");
                erros.Add((int)ErrosEnumeration.ReservaNaoExiste);
            }

            if (!erros.Any())
            {
                Reserva reserva = _reservaDAO.GetReserva(idReserva);
                if(reserva.Estado == EstadosReservaEnum.Pendente)
                {
                    reserva.Estado = decisao == true ? EstadosReservaEnum.Aceite : EstadosReservaEnum.Rejeitada;
                    reserva.IdFuncionarioDecide = idFuncionario;
                    if (decisao == true)
                    {
                        int numTelemovel = _clienteDAO.GetContaId(reserva.IdCliente).NumTelemovel;
                        ServiceResult<string> resultado = _pagamentoService.PedirPagamento(new MBWayPagamentoModel { NumTelemovel = numTelemovel, Valor = reserva.Preco });
                        if (!resultado.Sucesso)
                        {
                            _logger.LogWarning($"Ocorreu um erro no pedido de pagamento da Reserva com IdReserva {idReserva}.");
                            erros.Add((int)ErrosEnumeration.ErroNoPedidoDePagamento);
                        }
                        else
                        {
                            _logger.LogDebug($"A Reserva com IdReserva {idReserva} foi Aceitada pelo Funcionário com Funiconário {idFuncionario}.");
                            reserva.TransactionToken = resultado.Resultado;
                            _reservaDAO.EditarReserva(reserva);
                        }
                    }
                    else
                    {
                        _logger.LogDebug($"A Reserva com IdReserva {idReserva} foi Cancelada pelo Funcionário com Funiconário {idFuncionario}.");
                        _reservaDAO.EditarReserva(reserva);
                    }
                }
                else
                {
                    _logger.LogWarning($"A Reserva com IdReserva {idReserva} não pode transitar para o estado Aceitada/Rejeitada.");
                    erros.Add((int)ErrosEnumeration.TransicaoEstadosReservaImpossivel);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public IList<ReservaViewDTO> GetReservasEstado(EstadosReservaEnum estadosReserva)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> GetReservasEstado]");

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
                    itemViewDTO.ProdutoView.Url = new Uri($"{_appSettings.ServerUrl}/Images/Produto/{produto.IdProduto}.{produto.ExtensaoImagem}");
                    itensDTO.Add(itemViewDTO);
                }

                ReservaViewDTO reservaViewDTO = _mapper.Map<ReservaViewDTO>(reserva);
                reservaViewDTO.Itens = itensDTO;

                reservasViewDTO.Add(reservaViewDTO);
            }
            return reservasViewDTO;
        }



        public ServiceResult EntregarReserva(int idFuncionario, int idReserva)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> EntregarReserva]");

            IList<int> erros = new List<int>();

            if (!_funcionarioDAO.ExisteIdFuncionario(idFuncionario))
            {
                _logger.LogWarning($"Não existe nenhum Funcionário com o IdFuncionario {idFuncionario}.");
                erros.Add((int)ErrosEnumeration.ContaNaoExiste);
            }
            if (!_reservaDAO.ExisteReserva(idReserva))
            {
                _logger.LogWarning($"Não existe nenhuma Reserva com o IdReserva {idReserva}.");
                erros.Add((int)ErrosEnumeration.ReservaNaoExiste);
            }

            if (!erros.Any())
            {
                Reserva reserva = _reservaDAO.GetReserva(idReserva);
                if(reserva.Estado == EstadosReservaEnum.Paga)
                {
                    reserva.Estado = EstadosReservaEnum.Entregue;
                    _reservaDAO.EditarReserva(reserva);
                }
                else
                {
                    _logger.LogWarning($"A Reserva com IdReserva {idReserva} não pode transitar para o estado Entregue.");
                    erros.Add((int)ErrosEnumeration.TransicaoEstadosReservaImpossivel);
                }

            }

            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }




        private bool ValidaItens(IList<Item> itens)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> ValidaItens]");

            bool sucesso = true;

            foreach (Item item in itens)
            {
                if (item.Quantidade < 1 || (item.Observacoes != default && (item.Observacoes.Length <= 0 || item.Observacoes.Length > 300) || !_produtoDAO.ExisteProduto(item.IdProduto) || !_produtoDAO.IsAtivo(item.IdProduto)))
                {
                    sucesso = false;
                    break;
                }
            }

            return sucesso;
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
            return sucessoAbertura && sucessoEncerramento && horaEntrega.Date == now.Date && horaEntrega.TimeOfDay >= abertura.TimeOfDay && horaEntrega.TimeOfDay <= encerramento.TimeOfDay && (horaEntrega - now).TotalMinutes >= barSettings.TempoAprovocaoReserva;
        }

        private double CalculaValorTotalReserva(IList<Item> itens)
        {
            _logger.LogDebug("A executar [ReservaBusiness -> CalculaValorTotalReserva]");

            double precoTotal = 0;
            foreach (Item item in itens)
            {
                Produto produto = _produtoDAO.GetProduto(item.IdProduto);
                precoTotal += produto.Preco * item.Quantidade;
            }
            return Math.Round(precoTotal, 2);
        }

    }
}