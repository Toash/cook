using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;

/// <summary>
/// Pure data representation of a PreparedItem.
/// </summary>
public class PreparedItemData
{
    public Dictionary<IngredientType, int> Counts = new();


    public static PreparedItemData From(PreparedItem item)
    {
        var snap = new PreparedItemData();
        foreach (var ing in item.Ingredients)
        {
            if (snap.Counts.TryGetValue(ing.Type, out var count))
            {
                snap.Counts[ing.Type] = count + 1;
            }
            else
            {
                snap.Counts[ing.Type] = 1;
            }

        }
        return snap;
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