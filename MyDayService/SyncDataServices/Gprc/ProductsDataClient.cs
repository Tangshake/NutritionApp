using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using MyDayService.Dtos.Request;
using System.Threading.Tasks;
using ProductsCatalog;
using Grpc.Core;

namespace MyDayService.SyncDataServices.Grpc
{
    public class ProductsDataClient : IProductsDataClient
    {
        private readonly IConfiguration _configuration;

        public ProductsDataClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<GrpcResponseProductModel> ReturnAllProducts(GrpcRequestProductsModel grpcRequestProductsModel)
        {
            Console.WriteLine($"[Calling Grpc Serivce {_configuration["GrpcProducts"]}");
            
            var channel = GrpcChannel.ForAddress(_configuration["GrpcProducts"]);
            var client = new GrpcProducts.GrpcProductsClient(channel);
            var request = new GrpcRequestProductsModel(grpcRequestProductsModel);

            try
            {
                Console.WriteLine($"[ReturnAllProducts] Requesting Grpc message");
                var result = await client.GetAllProductsAsync(request);
                Console.WriteLine($"[ReturnAllProducts] Response Result: {result.Products.Count}");
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