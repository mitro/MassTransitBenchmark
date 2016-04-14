namespace DecisionEngine
{
    public interface IContextRunnerRegistry
    {
        IContextRunner CreateAndRegister();

        void Unregister(IContextRunner contextRunner);

        IContextRunner GetById(string contextId);
    }
}