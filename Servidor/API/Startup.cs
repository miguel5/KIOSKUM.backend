using System.Text;
using API.Business;
using API.Data;
using API.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using API.ViewModels;
using API.ViewModels.ClienteDTOs;
using API.Entities;
using Microsoft.Extensions.Logging;
using API.ViewModels.ProdutoDTOs;

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

            services.AddSingleton<IConnectionDB, ConnectionDB>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();

            services.AddScoped<IAdministradorDAO, AdministradorDAO>();
            services.AddScoped<ICategoriaDAO, CategoriaDAO>();
            services.AddSingleton<IClienteDAO, ClienteDAO>();
            services.AddScoped<IFuncionarioDAO, FuncionarioDAO>();
            services.AddScoped<IProdutoDAO, ProdutoDAO>();

            services.AddScoped<IAdministradorService, AdministradorService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddSingleton<IClienteService, ClienteService>();
            services.AddScoped<IFuncionarioService, FuncionarioService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Administrador, AdministradorDTO>();
                cfg.CreateMap<AdministradorDTO, Administrador>();
                cfg.CreateMap<Administrador, EditarAdministradorDTO>();
                cfg.CreateMap<EditarAdministradorDTO, Administrador>();

                cfg.CreateMap<Categoria, CategoriaDTO>();
                cfg.CreateMap<CategoriaDTO, Categoria>();

                cfg.CreateMap<Cliente, ClienteViewDTO>();
                cfg.CreateMap<ClienteViewDTO, Cliente>();
                cfg.CreateMap<Cliente, EditarClienteDTO>();
                cfg.CreateMap<EditarClienteDTO, Cliente>();


                cfg.CreateMap<Funcionario, FuncionarioDTO>();
                cfg.CreateMap<FuncionarioDTO, Funcionario>();

                cfg.CreateMap<RegistarProdutoDTO,Produto>();
                cfg.CreateMap<EditarProdutoDTO, Produto>();
                cfg.CreateMap<Produto, ProdutoViewDTO>();
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

            //app.UseHttpsRedirection();


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
