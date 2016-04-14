using System;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Engine.Contexts
{
    public class ContextRunner
    {
        public IBus Bus { get; set; }

        private readonly IContextStore _contextStore;

        public event Action<string> ContextFinished = delegate { };

        public ContextRunner(IContextStore contextStore)
        {
            _contextStore = contextStore;
        }

        public async Task Start(Context context)
        {
            context.StartedAt = DateTime.Now;
            context.ExecutedRules.Add(Rule.NoExecuted);

            await _contextStore.Insert(context);

            var executeFirstRule = new ExecuteRule(context.StartedAt, context.Id, RuleNumber.First);
            await Bus.Publish(executeFirstRule);
        }

        public async Task Process(string contextId, RuleExecuted rule)
        {
            var state = await _contextStore.GetLastRuleExecuted(contextId);

            if (state == Rule.SecondExecuted)
            {
                throw new Exception("No rule execution can be processed in a SecondExecuted state");
            }

            if (rule.Number == RuleNumber.First)
            {
                if (state != Rule.NoExecuted)
                {
                    throw new Exception("First rule execution can be processed only in a NoExecuted state");
                }

                var executeSecondRule = new ExecuteRule(DateTime.Now, contextId, RuleNumber.Second);

                await _contextStore.AddExecutedRule(contextId, Rule.FirstExecuted);

                await Bus.Publish(executeSecondRule);
            }
            else if (rule.Number == RuleNumber.Second)
            {
                if (state != Rule.FirstExecuted)
                {
                    throw new Exception("Second rule execution can be processed only in a FirstExecuted state");
                }

                await _contextStore.AddExecutedRule(contextId, Rule.SecondExecuted);
                await _contextStore.UpdateFinishedAt(contextId, DateTime.Now);

                ContextFinished(contextId);
            }
        }
    }
}