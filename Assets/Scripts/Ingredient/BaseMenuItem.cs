using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Food that the player sells, and what customers can order
/// </summary>

[CreateAssetMenu(fileName = "MenuItem", menuName = "Food/MenuItem")]
public class BaseMenuItem : ScriptableObject
{
    public string Name;
    public List<IngredientRequirement> Requirements = new List<IngredientRequirement>();
    public float BasePrice = 10;

    /// <summary>
    /// Returns counts of ingredients in a menu item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static Dictionary<IngredientData, int> Consolidate(BaseMenuItem item)
    {
        var map = new Dictionary<IngredientData, int>();

        foreach (IngredientRequirement req in item.Requirements)
        {
            if (!map.ContainsKey(req.Data))
                map[req.Data] = 0;

            map[req.Data] += req.Count;
        }

        return map;
    }
}