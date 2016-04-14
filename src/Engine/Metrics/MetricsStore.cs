using System;
using System.Threading;
using Engine.Contexts;

namespace Engine.Metrics
{
    public class MetricsStore
    {
        public int ContextCount { private get; set; }

        private int _finishedContexts;

        public event Action ExecutionCompleted = delegate { };

        public int FinishedContexts => _finishedContexts;

        public void LogContextFinish(string contextId)
        {
            Interlocked.Increment(ref _finishedContexts);

            if (_finishedContexts >= ContextCount)
            {
                ExecutionCompleted();
            }
        }
    }
}