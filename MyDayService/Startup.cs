using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MyDayService.AsyncDataServices.RabbitMQ.Log;
using MyDayService.AsyncDataServices.RabbitMQ.Subscriber;
using MyDayService.Dtos.RabbitMQ.Request;
using MyDayService.Entity;
using MyDayService.RabbitMQEventProcessing;
using MyDayService.Repository;
using MyDayService.SyncDataServices.Grpc;

namespace MyDayService
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

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            
            Console.WriteLine($"[ConfigureServices] MongoDBSettings. ConnectionString: {Configuration.GetConnectionString("MongoDbConnection")} DatabaseName: {Configuration.GetSection("MongoCollection")["Database"]}");
            services.AddSingleton( s=> 
                new MongoClient(Configuration.GetConnectionString("MongoDbConnection")).GetDatabase(Configuration.GetSection("MongoCollection")["Database"])
            );
            
            
            services.AddScoped<IDayOfEatingRepository, DayOfEatingRepository>();
            services.AddScoped<IProductsDataClient, ProductsDataClient>();
            services.AddScoped<IMealsDataClient, MealsDataClient>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<ILogMessageBusClient, LogMessageBusClient>();

            services.AddSingleton<IEventProcessor, EventProcessor>();
            services.AddHostedService<RabbitMQSubscriber>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyDayService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyDayService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
