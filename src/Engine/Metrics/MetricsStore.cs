using System;
using System.Threading;
using Engine.Contexts;

namespace Engine.Metrics
{
    public class MetricsStore
    {
        private int _contextCount;

        private int _finishedContexts;

        public event Action ExecutionCompleted = delegate { };

        public int FinishedContexts => _finishedContexts;

        public void LogContextFinish(string contextId)
        {
            Interlocked.Increment(ref _finishedContexts);

            if (_finishedContexts >= _contextCount)
            {
                ExecutionCompleted();
            }
        }

        public void Reset(int contextCount)
        {
            _contextCount = contextCount;
            _finishedContexts = 0;
        }
    }
}