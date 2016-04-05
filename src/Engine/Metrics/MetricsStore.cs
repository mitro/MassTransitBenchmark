using System;
using System.Threading;

namespace Engine.Metrics
{
    public class MetricsStore
    {
        private readonly int _contextCount;
        private int _finishedContexts;

        private object _lock = new object();

        public event Action ExecutionCompleted = delegate { };

        public MetricsStore(int contextCount)
        {
            _contextCount = contextCount;
        }

        public int FinishedContexts => _finishedContexts;

        public void LogContextFinish()
        {
            Interlocked.Increment(ref _finishedContexts);

            if (_finishedContexts >= _contextCount)
            {
                ExecutionCompleted();
            }
        }
    }
}