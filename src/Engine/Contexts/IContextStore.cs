using System;
using System.Collections.Generic;

namespace Engine.Contexts
{
    public interface IContextStore
    {
        void Insert(Context context);
        void Update(Context context);
        Context Get(string contextId);
        IEnumerable<Context> All();
        void Clear();
    }
}