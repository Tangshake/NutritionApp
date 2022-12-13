using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductsCatalog.AsyncDataService.RabbitMQ.Logs;
using ProductsCatalog.AsyncDataService.RabbitMQ.Product;
using ProductsCatalog.AsyncDataServices.Grpc;
using ProductsCatalog.Repositories;

namespace ProductsCatalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Console.WriteLine($"[Products] Application starting environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProductDatabaseAccess>( opt =>{
                opt.UseNpgsql(Configuration.GetSection("ConnectionStrings")["ProductConnection"]);
            });


            //Authentication and Authorization TEST
            //PACKAGES TO ADD:
            // dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 5.0.17
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "127.0.0.1",
                        ValidAudience = "127.0.0.1",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1111111111111111"))
                    };
                });

            services.AddScoped<IProductRepository, ProductRepository>();

            //RabbitMQ DI
            services.AddSingleton<ILogMessageBusClient, LogMessageBusClient>();
            services.AddSingleton<IProductMessageBusClient, ProductMessageBusClient>();

            services.AddGrpc();
            
            services.AddControllers(
                options => {
                    options.SuppressAsyncSuffixInActionNames = false;
                }
            ).AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductsCatalog", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductsCatalog v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcProductsService>();

                endpoints.MapGet("/Protos/products.proto", async context =>
                {
                    await context.Response.WriteAsync(File.ReadAllText("Protos/products.proto"));
                });
            });
        }
    }
}
