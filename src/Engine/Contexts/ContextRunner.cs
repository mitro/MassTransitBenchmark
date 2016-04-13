using System;
using System.Linq;
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

        public void Start(Context context)
        {
            context.StartedAt = DateTime.Now;
            context.ExecutedRules.Add(Rule.NoExecuted);

            _contextStore.Insert(context);

            var executeFirstRule = new ExecuteRule(context.StartedAt, context.Id, RuleNumber.First);
            Bus.Publish(executeFirstRule);
        }

        public void Process(string contextId, RuleExecuted rule)
        {
            var state = _contextStore.GetLastRuleExecuted(contextId);

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

                _contextStore.AddExecutedRule(contextId, Rule.FirstExecuted);

                Bus.Publish(executeSecondRule);
            }
            else if (rule.Number == RuleNumber.Second)
            {
                if (state != Rule.FirstExecuted)
                {
                    throw new Exception("Second rule execution can be processed only in a FirstExecuted state");
                }

                _contextStore.AddExecutedRule(contextId, Rule.SecondExecuted);
                _contextStore.UpdateFinishedAt(contextId, DateTime.Now);

                ContextFinished(contextId);
            }
        }
    }
}