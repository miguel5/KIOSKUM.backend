using System;
using System.Collections.Generic;
using System.Data;
using API.Business;
using API.Entities;
using MySql.Data.MySqlClient;

namespace API.Data
{
    public interface IProdutoDAO
    {
    }

    public class ProdutoDAO : IProdutoDAO
    {
        private readonly IConnectionDB _connectionDB;

        public ProdutoDAO(IConnectionDB connectionDB)
        {
            _connectionDB = connectionDB;
        }

    }
}
