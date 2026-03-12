using System;
using System.Collections.Generic;

public class NPCDialoguePresets
{
    public static Dictionary<OrderScoreCategories, List<string>> Sayings = new Dictionary<OrderScoreCategories, List<string>>()
    {
        {OrderScoreCategories.Bad,new List<string>{"Bad1","Bad2","Bad3"}},
        {OrderScoreCategories.Okay,new List<string>{"Okay1","Okay2","Okay3"}},
        {OrderScoreCategories.Good,new List<string>{"Good1","Good2","Good3"}},
    };

    public static string RandomEvaluationSaying(OrderScoreCategories category)
    {
        if (category == OrderScoreCategories.None) return "";
        var strings = Sayings[category];
        return strings[UnityEngine.Random.Range(0, strings.Count)];

    }

}
