using System.Collections.Generic;
using System.Threading.Tasks;
using MealsCatalog;
using MyDayService.Dtos.Request;
using ProductsCatalog;

namespace MyDayService.SyncDataServices.Grpc
{
    public interface IMealsDataClient
    {
        Task<GrpcResponseMealsModel> ReturnAllMeals(GrpcRequestMealsModel grpcRequestMealsModel);
    }
}