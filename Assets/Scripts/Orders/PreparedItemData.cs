using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;

/// <summary>
/// Pure data representation of a PreparedItem.
/// </summary>
public class PreparedItemData
{
    public Dictionary<IngredientData, int> Counts = new();


    // returns data from a prepared item
    public static PreparedItemData From(PreparedItem item)
    {
        var data = new PreparedItemData();
        foreach (var ing in item.Ingredients)
        {
            if (data.Counts.TryGetValue(ing.Data, out var count))
            {
                data.Counts[ing.Data] = count + 1;
            }
            else
            {
                data.Counts[ing.Data] = 1;
            }

        }
        return data;
    }

    public static List<PreparedItemData> From(List<PreparedItem> items)
    {
        var result = new List<PreparedItemData>(items.Count);

        foreach (var item in items)
        {
            result.Add(From(item));
        }

        return result;

    }
}