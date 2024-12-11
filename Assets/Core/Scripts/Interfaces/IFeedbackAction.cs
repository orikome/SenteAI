using System;

public interface IFeedbackAction
{
    Action OnSuccessCallback { get; set; }
    Action OnFailureCallback { get; set; }
    int SuccessCount { get; set; }
    int FailureCount { get; set; }
    float SuccessRate { get; set; }
    float FeedbackModifier { get; set; }
    void HandleSuccess(Agent agent);
    void HandleFailure(Agent agent);
    void UpdateSuccessRate();
    float ApplyFeedbackModifier(float utility, IFeedbackAction feedbackAction);
}
