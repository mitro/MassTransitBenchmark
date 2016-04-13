using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Agent
{
    public class ExecuteRuleConsumer: IConsumer<ExecuteRule>
    {

        public async Task Consume(ConsumeContext<ExecuteRule> context)
        {
            var message = context.Message;

            Console.WriteLine($"Executing rule for contextId={message.ContextId}, step={message.Number}, createdAt={message.CreatedAt}");

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