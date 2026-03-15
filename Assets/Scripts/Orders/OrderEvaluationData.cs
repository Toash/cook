using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public enum FinalOrderScoreCategory
{
    None,
    Bad,
    Okay,
    Good
}

[System.Serializable]
public class OrderScoreThreshold
{
    public FinalOrderScoreCategory Category;
    public float Threshold;
    public OrderScoreThreshold(FinalOrderScoreCategory category, float threshold)
    {
        this.Category = category;
        this.Threshold = threshold;

    }
}

[System.Serializable]
public class OrderScoreThresholds
{
    /// <summary>
    /// Thesholds should go from lowest to highest 
    /// </summary>
    public List<OrderScoreThreshold> MinimumThresholds;

    public FinalOrderScoreCategory GetCategoryFromScore(float score)
    {
        foreach (var threshold in MinimumThresholds.OrderByDescending(x => x.Threshold))
        {
            if (score >= threshold.Threshold)
            {
                return threshold.Category;
            }

        }
        return FinalOrderScoreCategory.None;

    }

}
/// <summary>
/// Settings for order evaluator
/// </summary>
[CreateAssetMenu(fileName = "OrderEvaluationData", menuName = "Orders/OrderEvaluationData")]
public class OrderEvaluationData : ScriptableObject
{
    public OrderScoreThresholds Thresholds;

}