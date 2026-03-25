public class FinalOrderEvaluationResult
{
    public float Score01;
    public FinalOrderScoreCategory ScoreCategory;
    public AssemblyEvaluation AssemblyEvaluation;
    public CookingEvaluation CookingEvaluation;
    public CondimentEvaluation CondimentEvaluation;
    public TimeEvaluation TimeEvaluation;

    public FinalOrderEvaluationResult(
        float score01,
        FinalOrderScoreCategory scoreCategory,
        AssemblyEvaluation assemblyEvaluation,
        CookingEvaluation cookingEvaluation,
        CondimentEvaluation condimentEvaluation,
        TimeEvaluation timeEvaluation)
    {
        Score01 = score01;
        ScoreCategory = scoreCategory;
        AssemblyEvaluation = assemblyEvaluation;
        CookingEvaluation = cookingEvaluation;
        CondimentEvaluation = condimentEvaluation;
        TimeEvaluation = timeEvaluation;
    }
}