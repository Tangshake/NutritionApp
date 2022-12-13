using System.Collections.Generic;
using System.Threading.Tasks;
using MyDayService.Dtos.Request;
using ProductsCatalog;

namespace MyDayService.SyncDataServices.Grpc
{
    public interface IProductsDataClient
    {
        Task<GrpcResponseProductModel> ReturnAllProducts(GrpcRequestProductsModel grpcRequestProductsModel);
    }
}