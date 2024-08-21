public abstract class ActionReadinessModule : AgentModule
{
    public abstract bool CanPerformAction(AgentAction action);
    public abstract void OnActionPerformed(AgentAction action);
}
