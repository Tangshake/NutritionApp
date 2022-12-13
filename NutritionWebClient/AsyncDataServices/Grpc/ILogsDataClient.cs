using System;
using System.Threading.Tasks;
using LogService;

namespace NutritionWebClient.AsyncDataServices.Grpc
{
    public interface ILogsDataClient
    {
        Task<GrpcResponseLogsDto> GetLogsByDateAndUserIdAsync(int userId, DateTime date);
    }
}