using System;
using System.Text.Json;
using System.Threading.Tasks;
using LogService.Dtos;
using LogService.Repositories;

namespace LogService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly ILogRepository _logRepository;

        public EventProcessor(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task ProcessEvent(string message)
        {
            var logDto = JsonSerializer.Deserialize<LogRequestDto>(message);

            Console.WriteLine($"[{logDto.Date}] [{logDto.ServiceName}] [{logDto.Method}] [{logDto.UserId}] [{logDto.Message}] [{logDto.Error}]");

            var result = await _logRepository.CreateLogAsync(logDto);

            Console.WriteLine($"[ProcessEvent] Inserted rows: {result}");
        }
    }
}
