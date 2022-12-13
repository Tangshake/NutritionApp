using System.Threading.Tasks;

namespace LogService.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string message);
    }
}