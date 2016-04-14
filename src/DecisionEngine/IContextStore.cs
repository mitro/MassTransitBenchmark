using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IContextStore
    {
        Task Save(Context context);

        Task<Context> Get(string contextId);
    }
}