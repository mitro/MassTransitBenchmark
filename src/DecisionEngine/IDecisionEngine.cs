using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IDecisionEngine
    {
        Task<ApplicationDecision> DecideOn(Application application);
    }
}