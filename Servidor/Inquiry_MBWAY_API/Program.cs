using System;
using System.Collections.Generic;
using System.IO;
using DAO;
using DAO.Interfaces;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Inquiry_MBWAY_API
{
    class Program
    {
        static void Main(string[] args)
        {

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            ConfigureLogger(configuration);

            var serviceCollection = ConfigureServices(configuration);

            using (var serviceProvider = serviceCollection.BuildServiceProvider())
            {
                Run(serviceProvider);
            }
        }

        private static void Run(IServiceProvider services)
        {
            Console.WriteLine("Entrei");
            var logger = services.GetService<ILogger<Program>>();

            var _reservaDAO = services.GetService<IReservaDAO>();

            
            IList<Reserva> listaReservasAceites = _reservaDAO.GetReservasEstado((int)EstadosReservaEnum.Aceite);

            Random random = new Random();
            foreach (Reserva reserva in listaReservasAceites)
            {
                var x = random.NextDouble();
                if (x <= 0.03)
                {
                    reserva.Estado = EstadosReservaEnum.Cancelada;
                    _reservaDAO.EditarReserva(reserva);
                }
                else if (x > 0.12)
                {
                    reserva.HoraPagamento = DateTime.Now;
                    reserva.Estado = EstadosReservaEnum.Paga;
                    _reservaDAO.EditarReserva(reserva);
                }
            }
        }

        private static IServiceCollection ConfigureServices(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IConfiguration>(configuration)
                .AddLogging(conf => conf.AddSerilog())
                .AddTransient<Services.DBConnection.IConnectionDBService, ConnectionDBService>()
                .AddTransient<IReservaDAO, ReservaDAO>();
            
            return serviceCollection;
        }

        private static void ConfigureLogger(IConfiguration configuration)
        {
            var loggingSection = configuration.GetSection("Logging");

            Log.Logger = new LoggerConfiguration()
               .WriteTo
               .RollingFile(loggingSection["PathFormat"])
               .CreateLogger();
        }
    }
}
