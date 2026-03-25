using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Manager for evaluating orders.
/// </summary>
public class OrderEvaluator : MonoBehaviour
{
    public static OrderEvaluator I;
    public OrderEvaluationData EvaluationData;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        else if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Evaluates an order and returns a result.
    /// </summary>
    public static FinalOrderEvaluationResult Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        if (I == null)
        {
            Debug.LogError("[OrderEvaluator]: is null");
            return null;
        }

        Debug.Log("[OrderEvaluator]: Evaluating order...");

        AssemblyEvaluation assemblyEvaluation = AssemblyEvaluation.Evaluate(order, preparedItems);
        CookingEvaluation cookingEvaluation = CookingEvaluation.Evaluate(order, preparedItems);
        CondimentEvaluation condimentEvaluation = CondimentEvaluation.Evaluate(order, preparedItems);

        TimeEvaluation timeEvaluation = new TimeEvaluation(
            completionTimeSeconds: order.TimeSinceOrdered,
            timeLimitSeconds: 60,
            fastThresholdRatio: .3f
        );

        float finalScore =
            assemblyEvaluation.Score01 * 0.4f +
            cookingEvaluation.Score01 * 0.4f +
            condimentEvaluation.Score01 * 0.2f;

        FinalOrderScoreCategory category = I.EvaluationData.Thresholds.GetCategoryFromScore(finalScore);

        return new FinalOrderEvaluationResult(
            finalScore,
            category,
            assemblyEvaluation,
            cookingEvaluation,
            condimentEvaluation,
            timeEvaluation
        );
    }
}