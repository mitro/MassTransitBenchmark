using DecisionEngine.Model;

namespace DecisionEngine.AgentResultConsumer
{
    public class AgentResultMessage
    {
        public AgentResult AgentResult { get; private set; }

        public AgentResultMessage(AgentResult agentResult)
        {
            AgentResult = agentResult;
        }
    }
}