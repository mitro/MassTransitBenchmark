﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Contexts
{
    public class ContextStore
    {
        private readonly ConcurrentDictionary<int, Context> _contexts = new ConcurrentDictionary<int, Context>();

        public void Add(Context context)
        {
            if (!_contexts.TryAdd(context.Id, context)) throw new Exception($"Context with id = {context.Id} already exists");
        }

        public Context Get(int contextId)
        {
            Context context;

            if (!_contexts.TryGetValue(contextId, out context)) throw new Exception($"Context with id={contextId} does not exist");

            return context;
        }

        public IEnumerable<Context> All()
        {
            return _contexts.Select(c => c.Value);
        }
    }
}