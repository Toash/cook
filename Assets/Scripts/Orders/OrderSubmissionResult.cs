public enum OrderSubmissionStatus
{
    Success,
    NoOrder
}

/// <summary>
/// Contains various information about what was decided for a particular order once the player submits it. 
/// </summary>
public readonly struct OrderSubmissionResult
{
    public readonly OrderSubmissionStatus Status;
    public readonly Order Order;
    public readonly OrderEvaluationResult Evaluation;
    public readonly float Payout;

    public OrderSubmissionResult(OrderSubmissionStatus status, Order order, OrderEvaluationResult evaluation, float Payout)
    {
        this.Status = status;
        this.Order = order;
        this.Evaluation = evaluation;
        this.Payout = Payout;
    }

}