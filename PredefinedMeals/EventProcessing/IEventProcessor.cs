using System.Threading.Tasks;

namespace PredefinedMeals.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEventAsync(string message);
    }
}