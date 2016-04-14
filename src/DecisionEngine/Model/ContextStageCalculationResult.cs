using System.Collections.Generic;

namespace DecisionEngine.Model
{
    public class ContextStageCalculationResult
    {
        public bool DecisionMade { get; private set; }

        public IEnumerable<AgentTask> AgentTasks { get; private set; }

        public ApplicationDecision Decision { get; set; }

        public ContextStageCalculationResult(bool decisionMade, IEnumerable<AgentTask> agentTasks, ApplicationDecision decision)
        {
            DecisionMade = decisionMade;
            AgentTasks = agentTasks;
            Decision = decision;
        }
    }
}