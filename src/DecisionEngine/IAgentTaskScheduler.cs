using System.Collections.Generic;
using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public interface IAgentTaskScheduler
    {
        Task Schedule(IEnumerable<AgentTask> agentTasks);
    }
}