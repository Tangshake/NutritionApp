using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NutritionWebClient.Dtos.Meal;
using NutritionWebClient.Dtos.Meal.Request;
using NutritionWebClient.Dtos.Meal.Response;

namespace NutritionWebClient.SyncDataService.Meals
{
    public interface IMealDataClient
    {
        Task<string> CreateMealAsync(PredefinedMealRequestDto predefinedMealRequestDto);

        Task<List<PredefinedMealResponseDto>> GetMealByNameAsync(int userId, string name);

        Task<PredefinedMealResponseDto> GetMealByIdAsync(int userId, string id);

        Task<List<PredefinedMealResponseDto>> GetAllUserMealsAsync(int userId);

        Task<bool> UpdateMealAsync(int userId, PredefinedMealUpdateRequestDto meal);

        Task<bool> RemoveMealAsync(Guid id, int userId);
    }
}