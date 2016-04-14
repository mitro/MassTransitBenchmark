using System.Threading.Tasks;
using DecisionEngine.Model;

namespace DecisionEngine
{
    public class ContextRunner : IContextRunner
    {
        private readonly IContextStageCalculator _contextStageCalculator;

        private readonly IAgentResultApplier _agentResultApplier;

        private readonly IAgentTaskScheduler _agentTaskScheduler;

        private readonly IContextStore _contextStore;

        private Context _context;

        private TaskCompletionSource<ApplicationDecision> _taskCompletionSource;

        public ContextRunner(IAgentTaskScheduler agentTaskScheduler, IContextStageCalculator contextStageCalculator, IAgentResultApplier agentResultApplier, IContextStore contextStore)
        {
            _agentTaskScheduler = agentTaskScheduler;
            _contextStageCalculator = contextStageCalculator;
            _agentResultApplier = agentResultApplier;
            _contextStore = contextStore;
        }

        public async Task<ApplicationDecision> RunToEnd(Context context)
        {
            _context = context;

            var result = await _contextStageCalculator.Calculate(context);

            await _contextStore.Save(context);

            if (result.DecisionMade) return result.Decision;

            await _agentTaskScheduler.Schedule(result.AgentTasks);

            _taskCompletionSource = new TaskCompletionSource<ApplicationDecision>(TaskCreationOptions.LongRunning);

            return await _taskCompletionSource.Task;
        }

        public async Task ProcessAgentResult(AgentResult agentResult)
        {
            var context = await _contextStore.Get(agentResult.ContextId);

            await _agentResultApplier.Apply(context, agentResult);

            var result = await _contextStageCalculator.Calculate(context);

            await _contextStore.Save(context);

            if (result.DecisionMade)
            {
                _taskCompletionSource.SetResult(result.Decision);
                return;
            }

            await _agentTaskScheduler.Schedule(result.AgentTasks);
        }
    }
}