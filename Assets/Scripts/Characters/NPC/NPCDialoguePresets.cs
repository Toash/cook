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

    public static List<string> ImpatientSayings = new List<string>()
    {
        "I've been waiting too long...",
        "This is taking forever.",
        "Forget it, I'm leaving.",
        "Nah, not worth the wait.",
        "I don't have time for this."
    };

    public static string RandomEvaluationSaying(FinalOrderScoreCategory category)
    {
        if (category == FinalOrderScoreCategory.None) return "";
        var strings = Sayings[category];
        return strings[UnityEngine.Random.Range(0, strings.Count)];
    }

    public static string RandomImpatientSaying()
    {
        return ImpatientSayings[UnityEngine.Random.Range(0, ImpatientSayings.Count)];
    }
}