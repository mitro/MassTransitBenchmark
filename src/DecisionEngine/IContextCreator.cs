using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IContextCreator
    {
        Task<Context> Create(Application application, Config config);
    }
}