using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Storage;
using System;
using System.Threading.Tasks;

namespace WhereHaveIBeen
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
            services.AddControllers();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = AuthenticationUtilities.GetIssuerKey()
                };
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var contextInfo = new ContextInfo
            {
                ConnectionString = Configuration["StorageConnectionString"]
            };
            var contextProvider = new ContextProvider(contextInfo);
            ContextProvider.Instance = contextProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ApplicationConfig.GoogleKey = Configuration["ApiKeys:Google"];

            AuthenticationUtilities.Issuer = Configuration["Jwt:Issuer"];
            AuthenticationUtilities.Audience = Configuration["Jwt:Audience"];
            AuthenticationUtilities.Signingkey = Configuration["Jwt:SigningKey"];

            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler("/Error");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private async Task InitStorage()
        {
            try
            {
                //await ContextProvider.Conn.CreateTableAsync<User>();
                //await ContextProvider.Conn.CreateTableAsync<Visit>();
                //await ContextProvider.Conn.CreateTableAsync<PersonAtRisk>();
            }
            catch (Exception ex)
            {
                var d = ex;
            }
        }
    }
}
