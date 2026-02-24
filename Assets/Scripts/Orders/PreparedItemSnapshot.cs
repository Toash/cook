using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;

/// <summary>
/// Pure data representation of a PreparedItem.
/// </summary>
public class PreparedItemSnapshot
{
    public Dictionary<FoodIngredientType, int> Counts = new();


    public static PreparedItemSnapshot From(PreparedItem item)
    {
        var snap = new PreparedItemSnapshot();
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

    public static List<PreparedItemSnapshot> From(List<PreparedItem> items)
    {
        var result = new List<PreparedItemSnapshot>(items.Count);

        foreach (var item in items)
        {
            result.Add(From(item));
        }

        return result;

    }
}