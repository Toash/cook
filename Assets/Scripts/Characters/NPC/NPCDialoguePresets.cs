using System;
using System.Collections.Generic;

public class NPCDialoguePresets
{
    public static Dictionary<FinalOrderScoreCategory, List<string>> Sayings = new Dictionary<FinalOrderScoreCategory, List<string>>()
    {
        {FinalOrderScoreCategory.Bad,new List<string>{"Bad1","Bad2","Bad3"}},
        {FinalOrderScoreCategory.Okay,new List<string>{"Okay1","Okay2","Okay3"}},
        {FinalOrderScoreCategory.Good,new List<string>{"Good1","Good2","Good3"}},
    };

    public static string RandomEvaluationSaying(FinalOrderScoreCategory category)
    {
        if (category == FinalOrderScoreCategory.None) return "";
        var strings = Sayings[category];
        return strings[UnityEngine.Random.Range(0, strings.Count)];

    }

}
