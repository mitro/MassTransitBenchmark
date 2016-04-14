namespace DecisionEngine.Model
{
    public class AgentTask
    {
        public string ContextId { get; private set; }

        public AgentTask(string contextId)
        {
            ContextId = contextId;
        }
    }
}