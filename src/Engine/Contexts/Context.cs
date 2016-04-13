using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Threading;
using Contracts;
using MassTransit;
using MongoDB.Bson.Serialization.Attributes;

namespace Engine.Contexts
{
    public class Context
    {
        [BsonId]
        public string Id { get; set; }

        public ContextState State { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public double ProcessingTimeInMs
        {
            get
            {
                if (State != ContextState.SecondRuleExecuted)
                {
                    throw new Exception($"Cannot calculate processing time because context {Id} is still running. Check your code.");
                }

                if (!FinishedAt.HasValue)
                {
                    throw new Exception($"Context {Id} has finished, but FinishedAt value was not set. Check your code.");
                }

                return (FinishedAt.Value - StartedAt).TotalMilliseconds;
            }
        }

        public Context()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public enum ContextState
    {
        Start,

        FirstRuleExecuted,

        SecondRuleExecuted
    }
}