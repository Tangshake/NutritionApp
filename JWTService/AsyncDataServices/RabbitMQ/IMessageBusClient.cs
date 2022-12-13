using JWTService.Dtos.RabbitMQ.Request;

namespace JWTService.AsyncDataServices.RabbitMQ
{
    public interface IMessageBusClient
    {
        void PublishNewLog(LogRequestDto log); 
    }
}