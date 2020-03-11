﻿using System;
using System.Collections.Generic;
using System.Text;

namespace API.Models
{
    public class Reserva
    {
        public int IdReserva { get; }
        public int IdCliente { get; set; }
        public Tuple<int, int> IdFuncionarios { get; set; }
        public IList<Tuple<int, int, string>> Items { get; set; } //(idProduto,Quantidade,Observações)
        public EstadosReservaEnum Estado { get; set; }
        public double Preco { get; }
        public DateTime HoraPagamento { get; set; }
        public DateTime HoraEntrega { get; }


        public Reserva(int IdReserva, int IdCliente, IList<Tuple<int, int, string>> Items, double Preco, DateTime HoraEntrega)
        {
            this.IdReserva = IdReserva;
            this.IdCliente = IdCliente;
            this.Items = Items;
            this.Estado = EstadosReservaEnum.Pendente;
            this.Preco = Preco;
            this.HoraEntrega = HoraEntrega;
        }


        public void AlteraEstadoReserva(int IdFuncionario, char decisao)
        {
            switch (decisao)
            {
                case 'a':
                    if (Estado == EstadosReservaEnum.Pendente)
                    {
                        IdFuncionarios = new Tuple<int, int>(IdFuncionario, -1);
                        Estado = EstadosReservaEnum.Aceite;
                    }
                    break;
                case 'r':
                    if (Estado == EstadosReservaEnum.Pendente)
                    {
                        IdFuncionarios = new Tuple<int, int>(IdFuncionario, -1);
                        Estado = EstadosReservaEnum.Rejeitada;
                    }
                    break;
                case 'e':
                    if (Estado == EstadosReservaEnum.Paga)
                    {
                        IdFuncionarios = new Tuple<int, int>(IdFuncionarios.Item1, IdFuncionario);
                        Estado = EstadosReservaEnum.Entregue;
                    }
                    break;
                default:
                    break;
            }
        }

        public void RegistaPagamento(DateTime HoraPagamento)
        {
            if (Estado == EstadosReservaEnum.Aceite)
            {
                this.Estado = EstadosReservaEnum.Paga;
                this.HoraPagamento = HoraPagamento;
            }
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 2;
                hash = hash * 31 + IdReserva;
                hash = hash * 31 + IdCliente;
                hash = hash * 31 + (IdFuncionarios == null ? 0 : IdFuncionarios.GetHashCode());
                hash = hash * 31 + (Items == null ? 0 : Items.GetHashCode());
                hash = hash * 31 + Estado.GetHashCode();
                hash = hash * 31 + Preco.GetHashCode();
                hash = hash * 31 + HoraPagamento.GetHashCode();
                hash = hash * 31 + HoraEntrega.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            Reserva reserva = (Reserva)obj;

            return IdReserva == reserva.IdReserva;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Reserva\n");
            sb.Append("- ID: " + IdReserva + "\n");
            sb.Append("- ID Cliente: " + IdCliente + "\n");
            if (Estado != EstadosReservaEnum.Pendente)
            {
                sb.Append("- ID Funcionário Confirma: " + IdFuncionarios.Item1 + "\n");
            }
            if (Estado == EstadosReservaEnum.Entregue)
            {
                sb.Append("- ID Funcionário Entrega: " + IdFuncionarios.Item2 + "\n");
            }
            sb.Append("- Items: \n");
            foreach (var item in Items)
            {
                sb.Append("\t* Id Produto: " + item.Item1 + ";Quantidade: " + item.Item2 + ";Observações: " + item.Item3 + ";\n");
            }
            sb.Append("- Estado: " + Estado + "\n");
            sb.Append("- Preco: " + Preco + "\n");
            sb.Append("- Hora Entrega: " + HoraEntrega + "\n");
            if (Estado == EstadosReservaEnum.Paga || Estado == EstadosReservaEnum.Entregue)
            {
                sb.Append("- Hora Pagamento: " + HoraPagamento + "\n");
            }
            return sb.ToString();
        }
    }
}
