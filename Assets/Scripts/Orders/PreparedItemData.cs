using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;

public class PreparedItemData
{
    public Dictionary<IngredientData, int> ConsolidatedCounts = new();
    public List<Ingredient> RuntimeIngredients = new();


    // returns data from a prepared item
    public static PreparedItemData From(PreparedItem item)
    {
        var data = new PreparedItemData();
        foreach (Ingredient ing in item.Ingredients)
        {
            if (data.ConsolidatedCounts.TryGetValue(ing.Data, out var count))
            {
                data.ConsolidatedCounts[ing.Data] = count + 1;

            }
            else
            {
                data.ConsolidatedCounts[ing.Data] = 1;
            }

        }
        data.RuntimeIngredients = item.Ingredients;
        return data;
    }

    public static List<PreparedItemData> From(List<PreparedItem> items)
    {
        var result = new List<PreparedItemData>(items.Count);

        foreach (PreparedItem item in items)
        {
            result.Add(From(item));
        }

        return result;

    }
}