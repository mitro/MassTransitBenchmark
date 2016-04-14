using System.Threading.Tasks;
using System.Xml;

namespace DecisionEngine.AgentResultConsumer
{
    public class AgentResultConsumer: IConsumer<AgentResultMessage>
    {
        private readonly IContextRunnerRegistry _contextRunnerRegistry;

        public AgentResultConsumer(IContextRunnerRegistry contextRunnerRegistry)
        {
            _contextRunnerRegistry = contextRunnerRegistry;
        }

        public async Task Consume(AgentResultMessage agentResultMessage)
        {
            var agentResult = agentResultMessage.AgentResult;

            var contextRunner = _contextRunnerRegistry.GetById(agentResult.ContextId);

            await contextRunner.ProcessAgentResult(agentResult);
        }
    }
}