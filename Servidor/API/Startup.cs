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
using API.Entities;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
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

            // configure DI for application services

            services.AddScoped<IConnectionDB, ConnectionDB>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();

            services.AddScoped<IAdministradorDAO, AdministradorDAO>();
            services.AddScoped<ICategoriaDAO, CategoriaDAO>();
            services.AddScoped<IClienteDAO, ClienteDAO>();
            services.AddScoped<IFuncionarioDAO, FuncionarioDAO>();
            services.AddScoped<IProdutoDAO, ProdutoDAO>();

            services.AddScoped<IAdministradorService, AdministradorService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IFuncionarioService, FuncionarioService>();
            services.AddScoped<IProdutoService, ProdutoService>();



            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Administrador, AdministradorDTO>();
                cfg.CreateMap<AdministradorDTO, Administrador>();
                cfg.CreateMap<Categoria, CategoriaDTO>();
                cfg.CreateMap<CategoriaDTO, Categoria>();
                cfg.CreateMap<Cliente, ClienteDTO>();
                cfg.CreateMap<ClienteDTO, Cliente>();
                cfg.CreateMap<Cliente, EditarClienteDTO>();
                cfg.CreateMap<EditarClienteDTO, Cliente>();
                cfg.CreateMap<Administrador, EditarAdministradorDTO>();
                cfg.CreateMap<EditarAdministradorDTO, Administrador>();
                cfg.CreateMap<Funcionario, FuncionarioDTO>();
                cfg.CreateMap<FuncionarioDTO, Funcionario>();
                cfg.CreateMap<Produto, ProdutoDTO>();
                cfg.CreateMap<ProdutoDTO, Produto>();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/mylog-{Date}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();


            // global cors policy
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
