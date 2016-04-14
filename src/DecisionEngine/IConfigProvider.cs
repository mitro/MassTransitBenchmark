using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IConfigProvider
    {
        Task<Config> GetConfig();
    }
}