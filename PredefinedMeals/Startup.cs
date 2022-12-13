using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PredefinedMeals.AsyncDataServices;
using PredefinedMeals.AsyncDataServices.Grpc;
using PredefinedMeals.AsyncDataServices.RabbitMQ;
using PredefinedMeals.EventProcessing;
using PredefinedMeals.Repositories;

namespace PredefinedMeals
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
            services.AddSingleton( s=> 
                new MongoClient(Configuration.GetConnectionString("MongoDbConnection")).GetDatabase(Configuration.GetSection("ServiceSettings")["ServiceName"])
            );
            
            services.AddGrpc();

            services.AddSingleton<IMealsRepository, MealsRepository>();
            services.AddSingleton<IEventProcessor, EventProcessor>();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();

            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddHostedService<RabbitMQSubscriber>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PredefinedMeals", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PredefinedMeals v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcMealsService>();

                endpoints.MapGet("/Protos/meals.proto", async context =>
                {
                    await context.Response.WriteAsync(File.ReadAllText("Protos/meals.proto"));
                });
            });
        }
    }
}
