using System.Threading.Tasks;

namespace DecisionEngine.AgentResultConsumer
{
    public interface IConsumer<in T>
    {
        Task Consume(T message);
    }
}