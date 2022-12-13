using ProductsCatalog.Dtos.RabbitMQ;

namespace ProductsCatalog.AsyncDataService.RabbitMQ.Product
{
    public interface IProductMessageBusClient
    {
        void PublishProductRemoved(int id);
    }
}