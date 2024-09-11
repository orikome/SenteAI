using System;

public interface IFeedbackAction
{
    Action OnSuccessCallback { get; set; }
    Action OnFailureCallback { get; set; }
    int SuccessCount { get; set; }
    int FailureCount { get; set; }
    float SuccessRate { get; set; }
    float FeedbackModifier { get; set; }
    void HandleSuccess(Enemy agent);
    void HandleFailure(Enemy agent);
    void UpdateSuccessRate();
    float ApplyFeedbackModifier(float utility, IFeedbackAction feedbackAction);
}
