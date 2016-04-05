using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Engine.Consumers
{
    public class ExecuteRuleConsumer: IConsumer<ExecuteRule>
    {
        public Task Consume(ConsumeContext<ExecuteRule> context)
        {
            var ruleExecuted = new RuleExecuted(DateTime.Now, context.Message.ContextId, context.Message.Number);
            context.Publish(ruleExecuted);
            return Task.FromResult(0);
        }
    }
}