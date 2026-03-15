
using System;
using System.Reflection;

/// <summary>
/// Evaluate an order to determine to what degree the PreparedItems matches the Menu Items of the order.
/// </summary>
public class FinalOrderEvaluationResult
{
    public float Score01;
    public FinalOrderScoreCategory ScoreCategory;
    public AssemblyEvaluation AssemblyEvaluation;
    public CookEvaluation CookEvaluation;
    // public int Discrepancies;


    public FinalOrderEvaluationResult(float Score, FinalOrderScoreCategory category, AssemblyEvaluation assemblyEvaluation)
    {
        this.Score01 = Math.Clamp(Score, 0, 1);
        this.ScoreCategory = category;
        this.AssemblyEvaluation = assemblyEvaluation;
    }

}