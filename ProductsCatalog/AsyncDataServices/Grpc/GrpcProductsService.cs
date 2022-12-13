using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using ProductsCatalog.Repositories;

namespace ProductsCatalog.AsyncDataServices.Grpc
{
    public class GrpcProductsService : GrpcProducts.GrpcProductsBase
    {
        private readonly IProductRepository _repository;

        public GrpcProductsService(IProductRepository repository)
        {
            _repository = repository;

        }

        public override async Task<GrpcResponseProductModel> GetAllProducts(GrpcRequestProductsModel grpcRequestProductsModel, ServerCallContext context)
        {
            Console.WriteLine($"[GrpcServerCall] GetAllProducts.");

            var ids = grpcRequestProductsModel.Ids.Select(i => i.ProductId_).ToList();
            var response = new GrpcResponseProductModel();
            var result = await _repository.GetProductsByIdAsync(ids);
            
            Console.WriteLine($"[GrpcServerCall] Products count: {result.Count()}.");

            foreach(var item in result)
                response.Products.Add(new ProductModel(item.AsGprcProductModel()));

            return response;
        }
    }
}