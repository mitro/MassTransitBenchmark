using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Engine.Consumers
{
    public class ExecuteRuleConsumer: IConsumer<ExecuteRule>
    {
        public async Task Consume(ConsumeContext<ExecuteRule> context)
        {
            var message = context.Message;

            if (message.Number == RuleNumber.First)
            {
                await Task.Delay(300);
            }
            else if (message.Number == RuleNumber.Second)
            {
                await Task.Delay(500);
            }

            var ruleExecuted = new RuleExecuted(DateTime.Now, message.ContextId, message.Number);

            await context.Publish(ruleExecuted);
        }
    }
}