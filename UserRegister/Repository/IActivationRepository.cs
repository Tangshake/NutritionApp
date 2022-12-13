using System.Threading.Tasks;
using UserRegister.Entity;

namespace UserRegister.Repository
{
    public interface IActivationRepository
    {
        Task<Activation> GetUserActivation(int userId, string token);
    }
}