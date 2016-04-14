using System;
using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IContextRunner
    {
        Task<ApplicationDecision> RunToEnd(Context context);

        Task ProcessAgentResult(AgentResult agentResult);
    }
}