using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ingredient.MenuItem;
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
    /// Evaluates an order and returns a reuslt
    /// 
    /// Given the list of Prepared Items, best matches the closes approximation. 
    ///     The closer it is- the higher the score. Does this for all of the prepared items.
    /// </summary>
    /// <param name="order"></param>
    /// <param name="preparedItems"></param>
    public static FinalOrderEvaluationResult Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        if (I == null)
        {
            Debug.LogError("[OrderEvaluator]: is null");
            return null;
        }
        AssemblyEvaluation assemblyEvaluation = AssemblyEvaluation.Evaluate(order, preparedItems);


        float finalScore = assemblyEvaluation.Score01;
        FinalOrderScoreCategory category = I.EvaluationData.Thresholds.GetCategoryFromScore(finalScore);




        return new FinalOrderEvaluationResult(finalScore, category, assemblyEvaluation);

    }



}