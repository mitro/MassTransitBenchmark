using System;
using System.Diagnostics;
using System.Threading;
using Contracts;
using MassTransit;

namespace Engine.Contexts
{
    public class Context
    {
        private static int _currentId;

        private ContextState _state;

        private Stopwatch _stopwatch;

        public int Id { get; }

        public IBus Bus { get; }

        public event Action ContextFinished = delegate { };

        public long ProcessingTimeInMs
        {
            get
            {
                if (_state != ContextState.SecondRuleExecuted) throw new Exception($"Cannot calculate processing time because context {Id} is still running");
                return _stopwatch.ElapsedMilliseconds;
            }
        }

        public Context(IBus bus)
        {
            Id = Interlocked.Increment(ref _currentId);
            Bus = bus;

            _state = ContextState.Start;
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _stopwatch.Start();

            var executeFirstRule = new ExecuteRule(DateTime.Now, Id, RuleNumber.First);
            Bus.Publish(executeFirstRule);
        }

        public void Process(RuleExecuted rule)
        {
            if (_state == ContextState.SecondRuleExecuted) throw new Exception("No rule execution cannot be processed in a SecondRuleExecuted state");

            if (rule.Number == RuleNumber.First)
            {
                if (_state != ContextState.Start) throw new Exception("First rule execution can be processed only in a Start state");

                _state = ContextState.FirstRuleExecuted;

                var executeSecondRule = new ExecuteRule(DateTime.Now, Id, RuleNumber.Second);
                Bus.Publish(executeSecondRule);
            }
            else if (rule.Number == RuleNumber.Second)
            {
                if (_state != ContextState.FirstRuleExecuted) throw new Exception("Second rule execution can be processed only in a FirstRuleExecuted state");

                _state = ContextState.SecondRuleExecuted;

                _stopwatch.Stop();

                ContextFinished();
            }
        }
    }

    public enum ContextState
    {
        Start,

        FirstRuleExecuted,

        SecondRuleExecuted
    }
}