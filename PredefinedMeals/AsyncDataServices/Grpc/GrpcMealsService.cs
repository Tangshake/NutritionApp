using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using MealsCatalog;
using Microsoft.Extensions.Configuration;
using PredefinedMeals.Repositories;

namespace PredefinedMeals.AsyncDataServices.Grpc
{
    public class GrpcMealsService : GrpcMeals.GrpcMealsBase
    {
        private readonly IMealsRepository _repository;

        public GrpcMealsService(IMealsRepository repository)
        {
            _repository = repository;
        }

        public override async Task<GrpcResponseMealsModel> GetAllMeals(GrpcRequestMealsModel grpcRequestMealsModel, ServerCallContext context)
        {
            Console.WriteLine($"[GrpcServerCall] GetAllProducts.");

            var ids = grpcRequestMealsModel.Ids.Select(i => i.MealId).ToList();
            var response = new GrpcResponseMealsModel();
            var result = await _repository.GetManyByIdAsync(ids);

            foreach(var item in result)
                 response.Meals.Add(new MealModel(item.AsGrpcMealModel()));

            return response;
        }
    }
}