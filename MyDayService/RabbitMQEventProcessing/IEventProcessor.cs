using System.Threading.Tasks;

namespace MyDayService.RabbitMQEventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEventAsync(string message);
    }
}