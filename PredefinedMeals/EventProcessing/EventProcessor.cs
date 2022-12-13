using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PredefinedMeals.Dtos.RabbitMQ.ProductPublished;
using PredefinedMeals.Repositories;

namespace PredefinedMeals.EventProcessing
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
                var repository = scope.ServiceProvider.GetRequiredService<IMealsRepository>();
                
                await repository.RemoveIngredientFromMeals(removedProductId);
            }
        }

        // private EventType DetermineEvent(string notificationMessage)
        // {
        //     Console.WriteLine("--> Determining Event");
        //     var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        //     Console.WriteLine($"--> Deserialized Event Name {eventType.EventType}");

        //     switch(eventType.EventType)
        //     {
        //         case "ProductChanged": 
        //             Console.WriteLine("--> ProductChanged"); 
        //             return EventType.ProductChanged;
        //         default:
        //             Console.WriteLine("--> Could not determine event type."); 
        //             return EventType.Undetermined;
        //     }
        // }

        // private void UpdatePredefinedMeals(string productPublishedMessage)
        // {
        //     using(var scope = _scopeFactory.CreateScope())
        //     {
        //         var repository = scope.ServiceProvider.GetRequiredService<IMealsRepository>();

        //         var productPublishedDto = JsonSerializer.Deserialize<ProductPublishedDto>(productPublishedMessage);

        //         Console.WriteLine("--> UPDATING PRDEFINED MEALS NOT IMPLEMENTED YET!");

        //         repository.UpdateIngredientAsync(productPublishedDto);
        //     }
        // }

    }

    // enum EventType
    // {
    //     ProductChanged,
    //     Undetermined
    // }

}