namespace SenteAI.Core
{
    public interface IAgentState
    {
        void Enter(Agent agent);
        void Execute(Agent agent);
        void Exit(Agent agent);
    }
}
