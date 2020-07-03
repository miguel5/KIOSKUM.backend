﻿using System.Collections.Generic;
using DAO.Interfaces;
using Entities;

namespace DAO
{
    public class ReservaDAO : IReservaDAO
    {
        public ReservaDAO()
        {
        }

        public void EditarReserva(Reserva reserva)
        {
            _logger.LogDebug("A executar [ReservaDAO -> EditarReserva]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "editar_reserva";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?idR", reserva.Idreserva);
                    cmd.Parameters["?idR"].Direction = ParameterDirection.Input;

                    if(reserva.HoraPagamento == default) {
                        cmd.Parameters.AddWithValue("?horaPaga", null);
                    }
                    else {
                        string hora = reserva.HoraPagamento.ToString("yyyy-MM-dd HH:mm:ss");
                        cmd.Parameters.AddWithValue("?horaPaga", hora);
                    }

                    cmd.Parameters["?horaPaga"].Direction = ParameterDirection.Input;

                    
                    cmd.Parameters.AddWithValue("?token", reserva.token);
                    cmd.Parameters["?token"].Direction = ParameterDirection.Input;


                    cmd.Parameters.AddWithValue("?idC", reserva.IdCliente);
                    cmd.Parameters["?idC"].Direction = ParameterDirection.Input;

                    if(reserva.IdFuncionarioDecide == default) {
                        cmd.Parameters.AddWithValue("?idAceita", null);
                    }
                    else {
                        cmd.Parameters.AddWithValue("?idAceita", reserva.IdFuncionarioDecide);
                    }

                    cmd.Parameters["?idAceita"].Direction = ParameterDirection.Input;


                    if(reserva.IdFuncionarioEntrega == default) {
                        cmd.Parameters.AddWithValue("?idAceita", null);
                    }
                    else {
                        cmd.Parameters.AddWithValue("?idEntrega", reserva.IdFuncionarioEntrega);
                    }

                    cmd.Parameters["?idEntrega"].Direction = ParameterDirection.Input;


                    cmd.ExecuteNonQuery();
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public bool ExisteReserva(int idReserva)
        {
            _logger.LogDebug("A executar [ReservaDAO -> ExisteReserva]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "existe_reserva";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idReserva);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    object val = cmd.ExecuteScalar();

                    return Convert.ToBoolean(val);
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public Reserva GetReserva(int idReserva)
        {
            _logger.LogDebug("A executar [ReservaDAO -> GetReserva]");
            try
            {
                _connectionDBService.OpenConnection();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "get_reserva";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?id", idReserva);
                    cmd.Parameters["?id"].Direction = ParameterDirection.Input;

                    using (MySqlDataReader var = cmd.ExecuteReader())
                    {
                        Reserva reserva = null;
                        if (var.Read())
                        {
                            reserva = new Reserva { IdReserva = idReserva, Itens = new List<Item>(), Estado = var.GetInt32(4), Preco = var.GetDouble(2), HoraEntrega = , HoraPagamento = , TransactionUniqueId = var.GetString(3    )};

                            var.Close();

                            cmd.Parameters.Clear();

                            cmd.CommandText = "get_ingredientes_produto";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                            cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;


                            using (MySqlDataReader varI = cmd.ExecuteReader())
                            {
                                while (varI.Read())
                                {
                                    produto.Ingredientes.Add(varI.GetString(0));
                                }
                            }

                            cmd.Parameters.Clear();

                            cmd.CommandText = "get_alergenios_produto";
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("?idProduto", produto.IdProduto);
                            cmd.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                            using (MySqlDataReader varA = cmd.ExecuteReader())
                            {
                                while (varA.Read())
                                {
                                    produto.Alergenios.Add(varA.GetString(0));
                                }
                            }
                        }
                        return produto;
                    }
                }
            }
            catch (Exception) { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }

        public List<Reserva> GetReservasEstado(int estado)
        {
            throw new System.NotImplementedException();
        }

        public void RegistarReserva(Reserva reserva)
        {
            try
            {
                _connectionDBService.OpenConnection();

                int reservaId;
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = _connectionDBService.Connection;

                    cmd.CommandText = "registar_reserva";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("?preco", reserva.Preco);
                    cmd.Parameters["?preco"].Direction = ParameterDirection.Input;

                    cmd.Parameters.AddWithValue("?estado", reserva.Estado);
                    cmd.Parameters["?estado"].Direction = ParameterDirection.Input;

                    string hora = reserva.HoraEntrega.ToString("yyyy-MM-dd HH:mm:ss");

                    cmd.Parameters.AddWithValue("?horaEntrega", hora);
                    cmd.Parameters["?horaEntrega"].Direction = ParameterDirection.Input;

                    reservaId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (MySqlCommand cmdU = new MySqlCommand())
                {
                    cmdU.Connection = _connectionDBService.Connection;

                    cmdU.CommandText = "registar_reserva_utilizadores";
                    cmdU.CommandType = CommandType.StoredProcedure;

                    cmdU.Parameters.AddWithValue("?idCliente", reserva.IdCliente);
                    cmdU.Parameters["?idCliente"].Direction = ParameterDirection.Input;

                    cmdU.Parameters.AddWithValue("?idReserva", reservaId);
                    cmdU.Parameters["?idReserva"].Direction = ParameterDirection.Input;

                    cmdU.ExecuteNonQuery();
                }

                foreach (Item item in reserva.Itens)
                {
                    using (MySqlCommand cmdI = new MySqlCommand())
                    {
                        cmdI.Connection = _connectionDBService.Connection;

                        cmdI.CommandText = "registar_reserva_produto";
                        cmdI.CommandType = CommandType.StoredProcedure;

                        cmdI.Parameters.AddWithValue("?idProduto", item.IdProduto);
                        cmdI.Parameters["?idProduto"].Direction = ParameterDirection.Input;

                        cmdI.Parameters.AddWithValue("?idReserva", reservaId);
                        cmdI.Parameters["?idReserva"].Direction = ParameterDirection.Input;

                        cmdI.Parameters.AddWithValue("?quantidade", item.Quantidade);
                        cmdI.Parameters["?quantidade"].Direction = ParameterDirection.Input;

                        cmdI.Parameters.AddWithValue("?observacoes", item.Observacoes);
                        cmdI.Parameters["?observacoes"].Direction = ParameterDirection.Input;

                        cmdI.ExecuteNonQuery();
                    }
                }
            }
            catch { throw; }
            finally
            {
                _connectionDBService.CloseConnection();
            }
        }
    }
}
