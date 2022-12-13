using System;
using System.Threading.Tasks;
using NutritionWebClient.Dtos.Doe.Response;
using NutritionWebClient.Dtos.Doe.Request;

namespace NutritionWebClient.SyncDataService.Does
{
    public interface IDoeDataClient
    {
        Task<DoeResponseDto> GetDoeByDateAsync(int userId, DateTime date);
        Task<bool> UpdateDoeAsync(int userId, DateTime date, DoeRequestDto doeRequestDto);
    }
}