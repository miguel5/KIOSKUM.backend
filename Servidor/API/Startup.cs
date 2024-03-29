using System.Text;
using Business.Interfaces;
using Business;
using DAO.Interfaces;
using Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using DTO.ClienteDTOs;
using Entities;
using Microsoft.Extensions.Logging;
using DTO.ProdutoDTOs;
using DTO.CategoriaDTOs;
using DAO;
using Services.DBConnection;
using Services.EmailSender;
using Services.Imagem;
using DTO.ReservaDTOs;
using DTO.TrabalhadorDTOs;
using Services.HashPassword;
using Services.Pagamentos;
using Services.Pagamentos.MBWay;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddTransient<IConnectionDBService, ConnectionDBService>();
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IHashPasswordService, HashPasswordService>();
            services.AddScoped<IImagemService, ImagemService>();
            services.AddScoped<IPagamentosService,MBWayService>();

            services.AddScoped<IAdministradorDAO, AdministradorDAO>();
            services.AddScoped<ICategoriaDAO, CategoriaDAO>();
            services.AddScoped<IClienteDAO, ClienteDAO>();
            services.AddScoped<IFuncionarioDAO, FuncionarioDAO>();
            services.AddScoped<IProdutoDAO, ProdutoDAO>();
            services.AddScoped<IReservaDAO, ReservaDAO>();

            services.AddScoped<IAdministradorBusiness, AdministradorBusiness>();
            services.AddScoped<ICategoriaBusiness, CategoriaBusiness>();
            services.AddScoped<IClienteBusiness, ClienteBusiness>();
            services.AddScoped<IFuncionarioBusiness, FuncionarioBusiness>();
            services.AddScoped<IProdutoBusiness, ProdutoBusiness>();
            services.AddScoped<IReservaBusiness, ReservaBusiness>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Administrador, TrabalhadorViewDTO>();
                cfg.CreateMap<TrabalhadorViewDTO, Administrador>();
                cfg.CreateMap<EditarTrabalhadorDTO, Administrador>();

                cfg.CreateMap<Funcionario, TrabalhadorViewDTO>();
                cfg.CreateMap<TrabalhadorViewDTO, Funcionario>();
                cfg.CreateMap<EditarTrabalhadorDTO, Funcionario>();

                cfg.CreateMap<RegistarCategoriaDTO, Categoria>();
                cfg.CreateMap<EditarCategoriaDTO, Categoria>();
                cfg.CreateMap<Categoria, CategoriaViewDTO>();


                cfg.CreateMap<Cliente, ClienteViewDTO>();
                cfg.CreateMap<ClienteViewDTO, Cliente>();
                cfg.CreateMap<EditarClienteDTO, Cliente>();


                cfg.CreateMap<RegistarProdutoDTO, Produto>();
                cfg.CreateMap<EditarProdutoDTO, Produto>();
                cfg.CreateMap<Produto, ProdutoViewDTO>();


                cfg.CreateMap<RegistarReservaDTO, Reserva>();
                cfg.CreateMap<Item, ItemViewDTO>();
                cfg.CreateMap<Reserva, ReservaViewDTO>();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

        }



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();


            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
