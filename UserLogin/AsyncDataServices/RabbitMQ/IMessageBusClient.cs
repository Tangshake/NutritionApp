using UserLogin.Dtos.RabbitMq;

namespace UserLogin.AsyncDataServices.RabbitMQ
{
    public interface IMessageBusClient
    {
       void PublishNewLog(LogDto log); 
    }
}