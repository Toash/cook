using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// runtime menu item
/// </summary>
public class OrderedMenuItem
{
    public BaseMenuItem BaseItem;

    public List<CondimentData> AddedCondiments = new();
    // public List<IngredientData> RemovedIngredients = new();
    // public List<IngredientData> ExtraIngredients = new();

    public OrderedMenuItem(BaseMenuItem baseItem)
    {
        BaseItem = baseItem;
    }
}