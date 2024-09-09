using System;

public interface IFeedbackAction
{
    Action OnSuccessCallback { get; set; }
    Action OnFailureCallback { get; set; }
    void HandleSuccess(Agent agent);
    void HandleFailure(Agent agent);
    void HandleMiss(Agent agent, float distanceToPlayer);
    float ApplyFeedbackModifier(float utility, IFeedbackAction feedbackAction);
}
