using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using LogService;
using Microsoft.Extensions.Configuration;

namespace NutritionWebClient.AsyncDataServices.Grpc
{
    public class LogsDataClient : ILogsDataClient
    {
        private readonly IConfiguration _configuration;

        public LogsDataClient(IConfiguration configuration)
        {
            _configuration = configuration;   
        }

        public async Task<GrpcResponseLogsDto> GetLogsByDateAndUserIdAsync(int userId, DateTime date)
        {
            var channel = GrpcChannel.ForAddress(_configuration["GrpcServices:Logs"]);
            var client = new GrpcLogs.GrpcLogsClient(channel);
            var request = new GrpcRequestLogDto() { UserId = userId, Date = Timestamp.FromDateTime(date) };

            try
            {
                var result = await client.GetLogsByDateAndUserIdAsync(request);

                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[There was a problem with Grpc Server call: {ex.Message}");
            }

            return null;
        }
    }
}