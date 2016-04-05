using System;
using System.Threading;

namespace Engine.Metrics
{
    public class MetricsStore
    {
        private readonly int _contextCount;

        private int _finishedContexts;
        private int _processedExecuteRule;
        private int _processedRuleExecuted;

        public event Action ExecutionCompleted = delegate { };

        public int FinishedContexts => _finishedContexts;
        public int ProcessedExecuteRule => _processedExecuteRule;
        public int ProcessedRuleExecuted => _processedRuleExecuted;

        public MetricsStore(int contextCount)
        {
            _contextCount = contextCount;
        }

        public void LogContextFinish()
        {
            Interlocked.Increment(ref _finishedContexts);

            if (_finishedContexts >= _contextCount)
            {
                ExecutionCompleted();
            }
        }

        public void LogExecuteRuleProcessed()
        {
            Interlocked.Increment(ref _processedExecuteRule);
        }

        public void LogRuleExecutedProcessed()
        {
            Interlocked.Increment(ref _processedRuleExecuted);
        }
    }
}