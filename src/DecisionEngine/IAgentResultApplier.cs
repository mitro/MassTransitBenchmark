using System.Collections.Generic;
using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IAgentResultApplier
    {
        Task Apply(Context context, AgentResult agentResult);
    }
}