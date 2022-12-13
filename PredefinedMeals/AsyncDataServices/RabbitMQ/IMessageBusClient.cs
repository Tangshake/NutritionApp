using PredefinedMeals.Dtos.RabbitMQ.Request;

namespace PredefinedMeals.AsyncDataServices.RabbitMQ
{
    public interface IMessageBusClient
    {
        void PublishNewLog(LogRequestDto log);
    }
}