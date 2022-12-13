using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using MyDayService.Dtos.Request;
using System.Threading.Tasks;
using ProductsCatalog;
using Grpc.Core;
using MealsCatalog;

namespace MyDayService.SyncDataServices.Grpc
{
    public class MealsDataClient : IMealsDataClient
    {
        private readonly IConfiguration _configuration;

        public MealsDataClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GrpcResponseMealsModel> ReturnAllMeals(GrpcRequestMealsModel grpcRequestMealsModel)
        {
            Console.WriteLine($"[Calling Grpc Meals Serivce {_configuration["GrpcMeals"]}");
            
            var channel = GrpcChannel.ForAddress(_configuration["GrpcMeals"]);
            var client = new GrpcMeals.GrpcMealsClient(channel);
            var request = new GrpcRequestMealsModel(grpcRequestMealsModel);

            try
            {
                Console.WriteLine($"[ReturnAllMeals] Requesting Grpc message");
                var result = await client.GetAllMealsAsync(request);
                Console.WriteLine($"[ReturnAllMeals] Response Result: {result.Meals.Count}");
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"There was problem with Grpc Server call: {ex.Message}");
                return null;
            }
        }
    }
}