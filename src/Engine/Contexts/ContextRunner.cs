using System;
using Contracts;
using MassTransit;

namespace Engine.Contexts
{
    public class ContextRunner
    {
        private readonly IBus _bus;

        private readonly IContextStore _contextStore;

        public event Action<Context> ContextFinished = delegate { };

        public ContextRunner(IBus bus, IContextStore contextStore)
        {
            _bus = bus;
            _contextStore = contextStore;
        }

        public void Start(Context context)
        {
            context.State = ContextState.Start;
            context.StartedAt = DateTime.Now;

            var executeFirstRule = new ExecuteRule(context.StartedAt, context.Id, RuleNumber.First);

            _contextStore.Insert(context);

            _bus.Publish(executeFirstRule);
        }

        public void Process(Context context, RuleExecuted rule)
        {
            var state = context.State;

            if (state == ContextState.SecondRuleExecuted)
            {
                throw new Exception("No rule execution cannot be processed in a SecondRuleExecuted state");
            }

            if (rule.Number == RuleNumber.First)
            {
                if (state != ContextState.Start)
                {
                    throw new Exception("First rule execution can be processed only in a Start state");
                }

                context.State = ContextState.FirstRuleExecuted;

                var executeSecondRule = new ExecuteRule(DateTime.Now, context.Id, RuleNumber.Second);

                _contextStore.Update(context);

                _bus.Publish(executeSecondRule);
            }
            else if (rule.Number == RuleNumber.Second)
            {
                if (state != ContextState.FirstRuleExecuted)
                {
                    throw new Exception("Second rule execution can be processed only in a FirstRuleExecuted state");
                }

                context.State = ContextState.SecondRuleExecuted;

                context.FinishedAt = DateTime.Now;

                _contextStore.Update(context);

                ContextFinished(context);
            }
        }
    }
}