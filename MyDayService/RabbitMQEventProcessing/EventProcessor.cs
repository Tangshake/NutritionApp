using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyDayService.Repository;

namespace MyDayService.RabbitMQEventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task ProcessEventAsync(string message)
        {
            Console.WriteLine("[ProcessEventAsync] Processing RabbitMQ message...");
            var removedProductId = JsonSerializer.Deserialize<int>(message);

            using(var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IDayOfEatingRepository>();
                
                await repository.RemoveIngredientFromDoeAsync(removedProductId);
            }
            Console.WriteLine("[ProcessEventAsync] Finished");
        }
    }
}