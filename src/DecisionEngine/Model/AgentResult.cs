namespace DecisionEngine.Model
{
    public class AgentResult
    {
        public string ContextId { get; private set; }

        public AgentResult(string contextId)
        {
            ContextId = contextId;
        }
    }
}