using System;
using System.Threading;
using System.Threading.Tasks;
using Contracts;
using Engine.Metrics;
using MassTransit;

namespace Engine.Consumers
{
    public class ExecuteRuleConsumer: IConsumer<ExecuteRule>
    {
        private readonly MetricsStore _metricsStore;

        public ExecuteRuleConsumer(MetricsStore metricsStore)
        {
            _metricsStore = metricsStore;
        }

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

            _metricsStore.LogExecuteRuleProcessed();
        }
    }
}